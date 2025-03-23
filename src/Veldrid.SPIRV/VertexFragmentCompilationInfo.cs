namespace Veldrid.SPIRV;

/// <summary>
/// The output of a cross-compile operation of a vertex and fragment shader from SPIR-V to some target language.
/// </summary>
public record VertexFragmentCompilationResult(
    string? VertexShader,
    string? FragmentShader,
    SpirvReflection? Reflection);
