﻿using System;
using System.Text;

namespace Veldrid.SPIRV;

/// <summary>
/// Static functions for cross-compiling SPIR-V bytecode to various shader languages, and for compiling GLSL to SPIR-V.
/// </summary>
public static class SpirvCompilation
{
    /// <summary>
    /// Cross-compiles the given vertex-fragment pair into some target language.
    /// </summary>
    /// <param name="vsBytes">The vertex shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="fsBytes">The fragment shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="target">The target language.</param>
    /// <returns>A <see cref="VertexFragmentCompilationResult"/> containing the compiled output.</returns>
    public static VertexFragmentCompilationResult CompileVertexFragment(
        byte[] vsBytes,
        byte[] fsBytes,
        CrossCompileTarget target
    ) => CompileVertexFragment(vsBytes, fsBytes, target, CrossCompileOptions.Default);

    /// <summary>
    /// Cross-compiles the given vertex-fragment pair into some target language.
    /// </summary>
    /// <param name="vsBytes">The vertex shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="fsBytes">The fragment shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="target">The target language.</param>
    /// <param name="options">The options for shader translation.</param>
    /// <returns>A <see cref="VertexFragmentCompilationResult"/> containing the compiled output.</returns>
    public static unsafe VertexFragmentCompilationResult CompileVertexFragment(
        byte[] vsBytes,
        byte[] fsBytes,
        CrossCompileTarget target,
        CrossCompileOptions options
    )
    {
        // int size1 = sizeof(CrossCompileInfo);
        // int size2 = sizeof(InteropArray);

        byte[] vsSpirvBytes;
        byte[] fsSpirvBytes;

        if (Util.HasSpirvHeader(vsBytes))
        {
            vsSpirvBytes = vsBytes;
        }
        else
        {
            fixed (byte* sourceTextPtr = vsBytes)
            {
                SpirvCompilationResult vsCompileResult = CompileGlslToSpirv(
                    (uint)vsBytes.Length,
                    sourceTextPtr,
                    string.Empty,
                    ShaderStages.Vertex,
                    target == CrossCompileTarget.GLSL || target == CrossCompileTarget.ESSL,
                    0,
                    null
                );
                vsSpirvBytes = vsCompileResult.SpirvBytes;
            }
        }

        if (Util.HasSpirvHeader(fsBytes))
        {
            fsSpirvBytes = fsBytes;
        }
        else
        {
            fixed (byte* sourceTextPtr = fsBytes)
            {
                SpirvCompilationResult fsCompileResult = CompileGlslToSpirv(
                    (uint)fsBytes.Length,
                    sourceTextPtr,
                    string.Empty,
                    ShaderStages.Fragment,
                    target == CrossCompileTarget.GLSL || target == CrossCompileTarget.ESSL,
                    0,
                    null
                );
                fsSpirvBytes = fsCompileResult.SpirvBytes;
            }
        }

        int specConstantsCount = options.Specializations.Length;
        NativeSpecializationConstant* nativeSpecConstants =
            stackalloc NativeSpecializationConstant[specConstantsCount];
        for (int i = 0; i < specConstantsCount; i++)
        {
            nativeSpecConstants[i].ID = options.Specializations[i].ID;
            nativeSpecConstants[i].Constant = options.Specializations[i].Data;
        }

        CrossCompileInfo info;
        info.Target = target;
        info.FixClipSpaceZ = options.FixClipSpaceZ;
        info.InvertY = options.InvertVertexOutputY;
        info.NormalizeResourceNames = options.NormalizeResourceNames;
        fixed (byte* vsBytesPtr = vsSpirvBytes)
        fixed (byte* fsBytesPtr = fsSpirvBytes)
        {
            info.VertexShader = new((uint)vsSpirvBytes.Length / 4, vsBytesPtr);
            info.FragmentShader = new((uint)fsSpirvBytes.Length / 4, fsBytesPtr);
            info.Specializations = new((uint)specConstantsCount, nativeSpecConstants);

            CompilationResult* result = null;
            try
            {
                result = VeldridSpirvNative.CrossCompile(&info);
                if (!result->Succeeded)
                {
                    throw new SpirvCompilationException(
                        "Compilation failed: "
                            + Util.GetString((byte*)result->GetData(0), result->GetLength(0))
                    );
                }

                string vsCode = Util.GetString((byte*)result->GetData(0), result->GetLength(0));
                string fsCode = Util.GetString((byte*)result->GetData(1), result->GetLength(1));

                ReflectionInfo* reflInfo = &result->ReflectionInfo;

                VertexElementDescription[] vertexElements = new VertexElementDescription[
                    reflInfo->VertexElements.Count
                ];
                for (uint i = 0; i < reflInfo->VertexElements.Count; i++)
                {
                    ref NativeVertexElementDescription nativeDesc =
                        ref reflInfo->VertexElements.Ref<NativeVertexElementDescription>(i);
                    vertexElements[i] = new(
                        Util.GetString((byte*)nativeDesc.Name.Data, nativeDesc.Name.Count),
                        nativeDesc.Semantic,
                        nativeDesc.Format,
                        nativeDesc.Offset
                    );
                }

                ResourceLayoutDescription[] layouts = new ResourceLayoutDescription[
                    reflInfo->ResourceLayouts.Count
                ];
                for (uint i = 0; i < reflInfo->ResourceLayouts.Count; i++)
                {
                    ref NativeResourceLayoutDescription nativeDesc =
                        ref reflInfo->ResourceLayouts.Ref<NativeResourceLayoutDescription>(i);
                    layouts[i].Elements = new ResourceLayoutElementDescription[
                        nativeDesc.ResourceElements.Count
                    ];
                    for (uint j = 0; j < nativeDesc.ResourceElements.Count; j++)
                    {
                        ref NativeResourceElementDescription elemDesc =
                            ref nativeDesc.ResourceElements.Ref<NativeResourceElementDescription>(
                                j
                            );
                        layouts[i].Elements[j] = new(
                            Util.GetString((byte*)elemDesc.Name.Data, elemDesc.Name.Count),
                            elemDesc.Kind,
                            elemDesc.Stages,
                            elemDesc.Options
                        );
                    }
                }

                SpirvReflection reflection = new(vertexElements, layouts);

                return new(vsCode, fsCode, reflection);
            }
            finally
            {
                if (result != null)
                {
                    VeldridSpirvNative.FreeResult(result);
                }
            }
        }
    }

    /// <summary>
    /// Cross-compiles the given vertex-fragment pair into some target language.
    /// </summary>
    /// <param name="csBytes">The compute shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="target">The target language.</param>
    /// <returns>A <see cref="ComputeCompilationResult"/> containing the compiled output.</returns>
    public static ComputeCompilationResult CompileCompute(
        byte[] csBytes,
        CrossCompileTarget target
    ) => CompileCompute(csBytes, target, CrossCompileOptions.Default);

    /// <summary>
    /// Cross-compiles the given vertex-fragment pair into some target language.
    /// </summary>
    /// <param name="csBytes">The compute shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="target">The target language.</param>
    /// <param name="options">The options for shader translation.</param>
    /// <returns>A <see cref="ComputeCompilationResult"/> containing the compiled output.</returns>
    public static unsafe ComputeCompilationResult CompileCompute(
        byte[] csBytes,
        CrossCompileTarget target,
        CrossCompileOptions options
    )
    {
        byte[] csSpirvBytes;

        if (Util.HasSpirvHeader(csBytes))
        {
            csSpirvBytes = csBytes;
        }
        else
        {
            fixed (byte* sourceTextPtr = csBytes)
            {
                SpirvCompilationResult vsCompileResult = CompileGlslToSpirv(
                    (uint)csBytes.Length,
                    sourceTextPtr,
                    string.Empty,
                    ShaderStages.Compute,
                    target == CrossCompileTarget.GLSL || target == CrossCompileTarget.ESSL,
                    0,
                    null
                );
                csSpirvBytes = vsCompileResult.SpirvBytes;
            }
        }

        CrossCompileInfo info;
        info.Target = target;
        info.FixClipSpaceZ = options.FixClipSpaceZ;
        info.InvertY = options.InvertVertexOutputY;
        info.NormalizeResourceNames = options.NormalizeResourceNames;
        fixed (byte* csBytesPtr = csSpirvBytes)
        fixed (SpecializationConstant* specConstants = options.Specializations)
        {
            info.ComputeShader = new((uint)csSpirvBytes.Length / 4, csBytesPtr);
            info.Specializations = new((uint)options.Specializations.Length, specConstants);

            CompilationResult* result = null;
            try
            {
                result = VeldridSpirvNative.CrossCompile(&info);
                if (!result->Succeeded)
                {
                    throw new SpirvCompilationException(
                        "Compilation failed: "
                            + Util.GetString((byte*)result->GetData(0), result->GetLength(0))
                    );
                }

                string csCode = Util.GetString((byte*)result->GetData(0), result->GetLength(0));

                ReflectionInfo* reflInfo = &result->ReflectionInfo;

                ResourceLayoutDescription[] layouts = new ResourceLayoutDescription[
                    reflInfo->ResourceLayouts.Count
                ];
                for (uint i = 0; i < reflInfo->ResourceLayouts.Count; i++)
                {
                    ref NativeResourceLayoutDescription nativeDesc =
                        ref reflInfo->ResourceLayouts.Ref<NativeResourceLayoutDescription>(i);
                    layouts[i].Elements = new ResourceLayoutElementDescription[
                        nativeDesc.ResourceElements.Count
                    ];
                    for (uint j = 0; j < nativeDesc.ResourceElements.Count; j++)
                    {
                        ref NativeResourceElementDescription elemDesc =
                            ref nativeDesc.ResourceElements.Ref<NativeResourceElementDescription>(
                                j
                            );
                        layouts[i].Elements[j] = new(
                            Util.GetString((byte*)elemDesc.Name.Data, elemDesc.Name.Count),
                            elemDesc.Kind,
                            elemDesc.Stages,
                            elemDesc.Options
                        );
                    }
                }

                SpirvReflection reflection = new([], layouts);

                return new(csCode, reflection);
            }
            finally
            {
                if (result != null)
                {
                    VeldridSpirvNative.FreeResult(result);
                }
            }
        }
    }

    /// <summary>
    /// Compiles the given GLSL source code into SPIR-V.
    /// </summary>
    /// <param name="sourceText">The shader source code.</param>
    /// <param name="fileName">A descriptive name for the shader. May be null.</param>
    /// <param name="stage">The <see cref="ShaderStages"/> which the shader is used in.</param>
    /// <param name="options">Parameters for the GLSL compiler.</param>
    /// <returns>A <see cref="SpirvCompilationResult"/> containing the compiled SPIR-V bytecode.</returns>
    public static unsafe SpirvCompilationResult CompileGlslToSpirv(
        string sourceText,
        string fileName,
        ShaderStages stage,
        GlslCompileOptions options
    )
    {
        int sourceAsciiCount = Encoding.ASCII.GetByteCount(sourceText);
        byte* sourceAsciiPtr = stackalloc byte[sourceAsciiCount];
        fixed (char* sourceTextPtr = sourceText)
        {
            Encoding.ASCII.GetBytes(
                sourceTextPtr,
                sourceText.Length,
                sourceAsciiPtr,
                sourceAsciiCount
            );
        }

        int macroCount = options.Macros.Length;
        NativeMacroDefinition* macros = stackalloc NativeMacroDefinition[macroCount];
        for (int i = 0; i < macroCount; i++)
        {
            macros[i] = new(options.Macros[i]);
        }

        return CompileGlslToSpirv(
            (uint)sourceAsciiCount,
            sourceAsciiPtr,
            fileName,
            stage,
            options.Debug,
            (uint)macroCount,
            macros
        );
    }

    internal static unsafe SpirvCompilationResult CompileGlslToSpirv(
        uint sourceLength,
        byte* sourceTextPtr,
        string? fileName,
        ShaderStages stage,
        bool debug,
        uint macroCount,
        NativeMacroDefinition* macros
    )
    {
        GlslCompileInfo info;
        info.Kind = GetShadercKind(stage);
        info.SourceText = new(sourceLength, sourceTextPtr);
        info.Debug = debug;
        info.Macros = new(macroCount, macros);

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "<veldrid-spirv-input>";
        }
        int fileNameAsciiCount = Encoding.ASCII.GetByteCount(fileName);
        byte* fileNameAsciiPtr = stackalloc byte[fileNameAsciiCount];
        if (fileNameAsciiCount > 0)
        {
            fixed (char* fileNameTextPtr = fileName)
            {
                Encoding.ASCII.GetBytes(
                    fileNameTextPtr,
                    fileName.Length,
                    fileNameAsciiPtr,
                    fileNameAsciiCount
                );
            }
        }
        info.FileName = new((uint)fileNameAsciiCount, fileNameAsciiPtr);

        CompilationResult* result = null;
        try
        {
            result = VeldridSpirvNative.CompileGlslToSpirv(&info);
            if (!result->Succeeded)
            {
                throw new SpirvCompilationException(
                    "Compilation failed: "
                        + Util.GetString((byte*)result->GetData(0), result->GetLength(0))
                );
            }

            uint length = result->GetLength(0);
            byte[] spirvBytes = new byte[(int)length];
            fixed (byte* spirvBytesPtr = &spirvBytes[0])
            {
                Buffer.MemoryCopy(result->GetData(0), spirvBytesPtr, length, length);
            }

            return new(spirvBytes);
        }
        finally
        {
            if (result != null)
            {
                VeldridSpirvNative.FreeResult(result);
            }
        }
    }

    static ShadercShaderKind GetShadercKind(ShaderStages stage)
    {
        return stage switch
        {
            ShaderStages.Vertex => ShadercShaderKind.Vertex,
            ShaderStages.Geometry => ShadercShaderKind.Geometry,
            ShaderStages.TessellationControl => ShadercShaderKind.TessellationControl,
            ShaderStages.TessellationEvaluation => ShadercShaderKind.TessellationEvaluation,
            ShaderStages.Fragment => ShadercShaderKind.Fragment,
            ShaderStages.Compute => ShadercShaderKind.Compute,
            _ => throw new SpirvCompilationException($"Invalid shader stage: {stage}"),
        };
    }
}
