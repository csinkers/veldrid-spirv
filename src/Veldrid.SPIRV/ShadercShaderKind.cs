namespace Veldrid.SPIRV;

/// <summary>
/// Shader types
/// </summary>
public enum ShadercShaderKind
{
    /// <summary>
    /// Vertex shader
    /// </summary>
    Vertex,

    /// <summary>
    /// Fragment shader
    /// </summary>
    Fragment,

    /// <summary>
    /// Compute shader
    /// </summary>
    Compute,

    /// <summary>
    /// Geometry shader
    /// </summary>
    Geometry,

    /// <summary>
    /// Tessellation control shader
    /// </summary>
    TessellationControl,

    /// <summary>
    /// Tessellation evaluation shader
    /// </summary>
    TessellationEvaluation,
}
