// ReSharper disable once CheckNamespace
namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Describes a single element of a vertex.
/// </summary>
public record struct VertexElementDescription(
    string Name,
    VertexElementSemantic Semantic,
    VertexElementFormat Format,
    uint Offset = 0);