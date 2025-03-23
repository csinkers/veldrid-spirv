namespace Veldrid.SPIRV;

/// <summary>
/// The output of a cross-compile operation of a compute shader from SPIR-V to some target language.
/// </summary>
public record ComputeCompilationResult(string ComputeShader, SpirvReflection Reflection);

