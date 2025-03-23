namespace Veldrid.SPIRV;

/// <summary>
/// An object used to control the options for compiling from GLSL to SPIR-V.
/// </summary>
public record GlslCompileOptions(bool Debug, MacroDefinition[] Macros);
