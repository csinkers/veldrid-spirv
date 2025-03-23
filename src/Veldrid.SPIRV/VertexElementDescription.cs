namespace Veldrid.SPIRV;

/// <summary>
/// Describes a single element of a vertex.
/// </summary>
public record struct VertexElementDescription(
    string Name,
    VertexElementSemantic Semantic,
    VertexElementFormat Format,
    uint Offset = 0);
