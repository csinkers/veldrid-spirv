using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Veldrid.SPIRV;

/// <summary>
/// Static functions for cross-compiling SPIR-V bytecode to various shader languages, and for compiling GLSL to SPIR-V.
/// </summary>
public static class SpirvCompilation
{
    /// <summary>
    /// Required when referencing the library with a ProjectReference rather than a PackageReference.
    /// </summary>
    public static void SetImportResolver()
    {
        var thisAssembly = Assembly.GetExecutingAssembly();
        NativeLibrary.SetDllImportResolver(thisAssembly, (name, _, _) =>
        {
            string baseDirectory = Path.GetDirectoryName(thisAssembly.Location)!;
            var (runtimeName, extension) = GetRuntimeName();
            var fullName = name + extension;
            string nativeDependencyPath = Path.Combine(baseDirectory, "runtimes", runtimeName, "native", fullName);

            if (!File.Exists(nativeDependencyPath))
                return IntPtr.Zero;

            if (NativeLibrary.TryLoad(nativeDependencyPath, out var libHandle))
                return libHandle;

            return IntPtr.Zero;
        });
    }

    static (string runtime, string extension) GetRuntimeName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return (RuntimeInformation.OSArchitecture == Architecture.X64 ? "win-x64" : "win-x86", ".dll");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return ("linux-x64", ".so");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return ("osx-x64", ".dylib");

        throw new PlatformNotSupportedException("Unsupported platform");
    }

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
        byte[] vsSpirvBytes = CompileInner(vsBytes, ShadercShaderKind.Vertex, target);
        byte[] fsSpirvBytes = CompileInner(fsBytes, ShadercShaderKind.Fragment, target);

        CrossCompileInfo info;
        info.Target = target;
        info.FixClipSpaceZ = options.FixClipSpaceZ;
        info.InvertY = options.InvertVertexOutputY;
        info.NormalizeResourceNames = options.NormalizeResourceNames;

        Span<NativeSpecializationConstant> specConstants = stackalloc NativeSpecializationConstant[options.Specializations.Length];
        for (int i = 0; i < options.Specializations.Length; i++)
            specConstants[i] = options.Specializations[i].ToNative();

        fixed (byte* vsBytesPtr = vsSpirvBytes)
        fixed (byte* fsBytesPtr = fsSpirvBytes)
        fixed (NativeSpecializationConstant* pConstants = &specConstants[0])
        {
            info.VertexShader = new((uint)vsSpirvBytes.Length / 4, vsBytesPtr);
            info.FragmentShader = new((uint)fsSpirvBytes.Length / 4, fsBytesPtr);
            info.Specializations = new((uint)specConstants.Length, pConstants);

            CompilationResult* result = null;
            try
            {
                result = VeldridSpirvNative.CrossCompile(&info);
                if (!result->Succeeded)
                    throw new SpirvCompilationException("Compilation failed: " + SpirvUtil.GetString(result->Data(0)));

                string vsCode = SpirvUtil.GetString(result->Data(0));
                string fsCode = SpirvUtil.GetString(result->Data(1));

                ReflectionInfo* reflInfo = &result->ReflectionInfo;
                var vertexElements = new VertexElementDescription[reflInfo->VertexElements.Count];
                for (uint i = 0; i < reflInfo->VertexElements.Count; i++)
                    vertexElements[i] = reflInfo->VertexElements[i].ToManaged();

                var layouts = new ResourceLayoutDescription[reflInfo->ResourceLayouts.Count];
                for (uint i = 0; i < reflInfo->ResourceLayouts.Count; i++)
                {
                    ref NativeResourceLayoutDescription nativeDesc = ref reflInfo->ResourceLayouts[i];

                    layouts[i].Elements = new ResourceLayoutElementDescription[nativeDesc.ResourceElements.Count];
                    for (uint j = 0; j < nativeDesc.ResourceElements.Count; j++)
                        layouts[i].Elements[j] = nativeDesc.ResourceElements[j].ToManaged();
                }

                SpirvReflection reflection = new(vertexElements, layouts);
                return new(vsCode, fsCode, reflection);
            }
            finally
            {
                if (result != null)
                    VeldridSpirvNative.FreeResult(result);
            }
        }
    }

    /// <summary>
    /// Cross-compiles the given vertex-fragment pair into some target language.
    /// </summary>
    /// <param name="csBytes">The compute shader's SPIR-V bytecode or ASCII-encoded GLSL source code.</param>
    /// <param name="target">The target language.</param>
    /// <returns>A <see cref="ComputeCompilationResult"/> containing the compiled output.</returns>
    public static ComputeCompilationResult CompileCompute(byte[] csBytes, CrossCompileTarget target)
        => CompileCompute(csBytes, target, CrossCompileOptions.Default);

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
        byte[] csSpirvBytes = CompileInner(csBytes, ShadercShaderKind.Compute, target);

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
                    throw new SpirvCompilationException("Compilation failed: " + SpirvUtil.GetString(result->Data(0)));

                string csCode = SpirvUtil.GetString(result->Data(0));

                ReflectionInfo* reflInfo = &result->ReflectionInfo;
                var layouts = new ResourceLayoutDescription[reflInfo->ResourceLayouts.Count];

                for (uint i = 0; i < reflInfo->ResourceLayouts.Count; i++)
                {
                    ref NativeResourceLayoutDescription nativeDesc = ref reflInfo->ResourceLayouts[i];
                    layouts[i].Elements = new ResourceLayoutElementDescription[nativeDesc.ResourceElements.Count];
                    for (uint j = 0; j < nativeDesc.ResourceElements.Count; j++)
                        layouts[i].Elements[j] = nativeDesc.ResourceElements[j].ToManaged();
                }

                SpirvReflection reflection = new([], layouts);
                return new(csCode, reflection);
            }
            finally
            {
                if (result != null)
                    VeldridSpirvNative.FreeResult(result);
            }
        }
    }

    /// <summary>
    /// Compiles the given GLSL source code into SPIR-V.
    /// </summary>
    /// <param name="sourceText">The shader source code.</param>
    /// <param name="fileName">A descriptive name for the shader. May be null.</param>
    /// <param name="kind">The <see cref="ShadercShaderKind"/> which the shader is used in.</param>
    /// <param name="options">Parameters for the GLSL compiler.</param>
    /// <returns>A <see cref="SpirvCompilationResult"/> containing the compiled SPIR-V bytecode.</returns>
    public static unsafe SpirvCompilationResult CompileGlslToSpirv(
        string sourceText,
        string fileName,
        ShadercShaderKind kind,
        GlslCompileOptions options)
    {
        int sourceAsciiCount = Encoding.ASCII.GetByteCount(sourceText);
        Span<byte> sourceAscii = stackalloc byte[sourceAsciiCount];
        Encoding.ASCII.GetBytes(sourceText.AsSpan(), sourceAscii);

        return CompileGlslToSpirv(sourceAscii, fileName, kind, options);
    }

    /// <summary>
    /// Compiles the given GLSL source code into SPIR-V.
    /// </summary>
    /// <param name="sourceText">The shader source code.</param>
    /// <param name="fileName">A descriptive name for the shader. May be null.</param>
    /// <param name="kind">The <see cref="ShadercShaderKind"/> which the shader is used in.</param>
    /// <param name="options">Parameters for the GLSL compiler.</param>
    /// <returns>A <see cref="SpirvCompilationResult"/> containing the compiled SPIR-V bytecode.</returns>
    public static unsafe SpirvCompilationResult CompileGlslToSpirv(
        ReadOnlySpan<byte> sourceText,
        string? fileName,
        ShadercShaderKind kind,
        GlslCompileOptions options)
    {
        var macros = options.Macros.AsSpan();
        NativeMacroDefinition* nativeMacros = stackalloc NativeMacroDefinition[macros.Length];
        for (int i = 0; i < macros.Length; i++)
            nativeMacros[i] = new(macros[i]);

        fixed (byte* sourcePtr = &sourceText[0])
        {
            GlslCompileInfo info;
            info.Kind = kind;
            info.SourceText = new InteropArray<byte>((uint)sourceText.Length, sourcePtr);
            info.Debug = options.Debug;
            info.Macros = new((uint)macros.Length, nativeMacros);

            if (string.IsNullOrEmpty(fileName))
                fileName = "<veldrid-spirv-input>";

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
                    throw new SpirvCompilationException("Compilation failed: " + SpirvUtil.GetString(result->Data(0)));

                byte[] spirvBytes = result->Data(0).ToArray();
                return new(spirvBytes);
            }
            finally
            {
                if (result != null)
                    VeldridSpirvNative.FreeResult(result);
            }
        }
    }

    static byte[] CompileInner(byte[] shaderBytes, ShadercShaderKind kind, CrossCompileTarget target)
    {
        if (SpirvUtil.HasSpirvHeader(shaderBytes))
            return shaderBytes;

        SpirvCompilationResult vsCompileResult = CompileGlslToSpirv(
            shaderBytes,
            string.Empty,
            kind,
            new GlslCompileOptions(target is CrossCompileTarget.GLSL or CrossCompileTarget.ESSL, []));

        return vsCompileResult.SpirvBytes;
    }
}
