﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Veldrid.SPIRV
{
    public class VariantStageDescription
    {
        public ShaderStages Stage { get; }
        public string FileName { get; }

        public VariantStageDescription(ShaderStages stage, string fileName)
        {
            Stage = stage;
            FileName = fileName;
        }
    }

    public class ShaderVariantDescription
    {
        public string Name { get; }
        public VariantStageDescription[] Shaders { get; }
        public MacroDefinition[] Macros { get; }
        public CrossCompileOptions CrossCompileOptions { get; }
        public CrossCompileTarget[] Targets { get; }

        public ShaderVariantDescription(
            string name,
            VariantStageDescription[] shaders,
            MacroDefinition[] macros,
            CrossCompileOptions crossCompileOptions,
            CrossCompileTarget[] targets
        )
        {
            Name = name;
            Shaders = shaders;
            Macros = macros;
            CrossCompileOptions = crossCompileOptions;
            Targets = targets;
        }
    }

    public class VariantCompiler
    {
        internal static JsonSerializerOptions JsonOptions { get; }

        static VariantCompiler()
        {
            JsonOptions = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
            JsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private readonly List<string> _shaderSearchPaths = new();
        private readonly string _outputPath;

        public VariantCompiler(List<string> shaderSearchPaths, string outputPath)
        {
            _shaderSearchPaths = shaderSearchPaths;
            _outputPath = outputPath;
        }

        public string[] Compile(ShaderVariantDescription variant)
        {
            if (variant.Shaders.Length == 1)
            {
                if (variant.Shaders[0].Stage == ShaderStages.Vertex)
                {
                    return CompileVertexFragment(variant);
                }
                if (variant.Shaders[0].Stage == ShaderStages.Compute)
                {
                    return CompileCompute(variant);
                }
            }
            if (variant.Shaders.Length == 2)
            {
                bool hasVertex = false;
                bool hasFragment = false;
                foreach (VariantStageDescription shader in variant.Shaders)
                {
                    hasVertex |= shader.Stage == ShaderStages.Vertex;
                    hasFragment |= shader.Stage == ShaderStages.Fragment;
                }

                if (!hasVertex)
                {
                    throw new SpirvCompilationException(
                        $"Variant \"{variant.Name}\" is missing a vertex shader."
                    );
                }
                if (!hasFragment)
                {
                    throw new SpirvCompilationException(
                        $"Variant \"{variant.Name}\" is missing a fragment shader."
                    );
                }

                return CompileVertexFragment(variant);
            }
            else
            {
                throw new SpirvCompilationException(
                    $"Variant \"{variant.Name}\" has an unsupported combination of shader stages."
                );
            }
        }

        private string[] CompileVertexFragment(ShaderVariantDescription variant)
        {
            List<string> generatedFiles = new();
            List<Exception> compilationExceptions = new();
            byte[] vsBytes = null;
            byte[] fsBytes = null;

            string vertexFileName = variant
                .Shaders.FirstOrDefault(vsd => vsd.Stage == ShaderStages.Vertex)
                ?.FileName;
            if (vertexFileName != null)
            {
                try
                {
                    vsBytes = CompileToSpirv(variant, vertexFileName, ShaderStages.Vertex);
                    string spvPath = Path.Combine(
                        _outputPath,
                        $"{variant.Name}_{ShaderStages.Vertex}.spv"
                    );
                    File.WriteAllBytes(spvPath, vsBytes);
                    generatedFiles.Add(spvPath);
                }
                catch (Exception e)
                {
                    compilationExceptions.Add(e);
                }
            }

            string fragmentFileName = variant
                .Shaders.FirstOrDefault(vsd => vsd.Stage == ShaderStages.Fragment)
                ?.FileName;
            if (fragmentFileName != null)
            {
                try
                {
                    fsBytes = CompileToSpirv(variant, fragmentFileName, ShaderStages.Fragment);
                    string spvPath = Path.Combine(
                        _outputPath,
                        $"{variant.Name}_{ShaderStages.Fragment}.spv"
                    );
                    File.WriteAllBytes(spvPath, fsBytes);
                    generatedFiles.Add(spvPath);
                }
                catch (Exception e)
                {
                    compilationExceptions.Add(e);
                }
            }

            if (compilationExceptions.Count > 0)
            {
                throw new AggregateException(
                    $"Errors were encountered when compiling from GLSL to SPIR-V.",
                    compilationExceptions
                );
            }

            foreach (CrossCompileTarget target in variant.Targets)
            {
                try
                {
                    bool writeReflectionFile = true;
                    VertexFragmentCompilationResult result = SpirvCompilation.CompileVertexFragment(
                        vsBytes,
                        fsBytes,
                        target,
                        variant.CrossCompileOptions
                    );
                    if (result.VertexShader != null)
                    {
                        string vsPath = Path.Combine(
                            _outputPath,
                            $"{variant.Name}_Vertex.{GetExtension(target)}"
                        );
                        File.WriteAllText(vsPath, result.VertexShader);
                        generatedFiles.Add(vsPath);
                    }
                    if (result.FragmentShader != null)
                    {
                        string fsPath = Path.Combine(
                            _outputPath,
                            $"{variant.Name}_Fragment.{GetExtension(target)}"
                        );
                        File.WriteAllText(fsPath, result.FragmentShader);
                        generatedFiles.Add(fsPath);
                    }

                    if (writeReflectionFile)
                    {
                        writeReflectionFile = false;
                        string reflectionPath = Path.Combine(
                            _outputPath,
                            $"{variant.Name}_ReflectionInfo.json"
                        );

                        using (Stream sw = File.Create(reflectionPath))
                        {
                            JsonSerializer.Serialize(sw, result.Reflection, JsonOptions);
                        }
                        generatedFiles.Add(reflectionPath);
                    }
                }
                catch (Exception e)
                {
                    compilationExceptions.Add(e);
                }
            }

            if (compilationExceptions.Count > 0)
            {
                throw new AggregateException(
                    $"Errors were encountered when compiling shader variant(s).",
                    compilationExceptions
                );
            }

            return generatedFiles.ToArray();
        }

        private string GetExtension(CrossCompileTarget target)
        {
            return target switch
            {
                CrossCompileTarget.HLSL => "hlsl",
                CrossCompileTarget.GLSL => "glsl",
                CrossCompileTarget.ESSL => "essl",
                CrossCompileTarget.MSL => "metal",
                _ => throw new SpirvCompilationException($"Invalid CrossCompileTarget: {target}"),
            };
        }

        private byte[] CompileToSpirv(
            ShaderVariantDescription variant,
            string fileName,
            ShaderStages stage
        )
        {
            GlslCompileOptions glslOptions = GetOptions(variant);
            string glsl = LoadGlsl(fileName);
            SpirvCompilationResult result = SpirvCompilation.CompileGlslToSpirv(
                glsl,
                fileName,
                stage,
                glslOptions
            );
            return result.SpirvBytes;
        }

        private GlslCompileOptions GetOptions(ShaderVariantDescription variant)
        {
            return new GlslCompileOptions(false, variant.Macros);
        }

        private string LoadGlsl(string fileName)
        {
            if (fileName == null)
            {
                return null;
            }

            foreach (string searchPath in _shaderSearchPaths)
            {
                string fullPath = Path.Combine(searchPath, fileName);
                if (File.Exists(fullPath))
                {
                    return File.ReadAllText(fullPath);
                }
            }

            throw new FileNotFoundException($"Unable to find shader file \"{fileName}\".");
        }

        private string[] CompileCompute(ShaderVariantDescription variant)
        {
            List<string> generatedFiles = new();
            byte[] csBytes = CompileToSpirv(
                variant,
                variant.Shaders[0].FileName,
                ShaderStages.Compute
            );
            string spvPath = Path.Combine(
                _outputPath,
                $"{variant.Name}_{ShaderStages.Compute}.spv"
            );
            File.WriteAllBytes(spvPath, csBytes);
            generatedFiles.Add(spvPath);

            List<Exception> compilationExceptions = new();
            foreach (CrossCompileTarget target in variant.Targets)
            {
                try
                {
                    ComputeCompilationResult result = SpirvCompilation.CompileCompute(
                        csBytes,
                        target,
                        variant.CrossCompileOptions
                    );
                    string csPath = Path.Combine(
                        _outputPath,
                        $"{variant.Name}_Compute.{GetExtension(target)}"
                    );
                    File.WriteAllText(csPath, result.ComputeShader);
                    generatedFiles.Add(csPath);

                    string reflectionPath = Path.Combine(
                        _outputPath,
                        $"{variant.Name}_ReflectionInfo.json"
                    );

                    using (Stream sw = File.Create(reflectionPath))
                    {
                        JsonSerializer.Serialize(sw, result.Reflection, JsonOptions);
                    }
                    generatedFiles.Add(reflectionPath);
                }
                catch (Exception e)
                {
                    compilationExceptions.Add(e);
                }
            }

            if (compilationExceptions.Count > 0)
            {
                throw new AggregateException(
                    $"Errors were encountered when compiling shader variant(s).",
                    compilationExceptions
                );
            }

            return generatedFiles.ToArray();
        }
    }
}
