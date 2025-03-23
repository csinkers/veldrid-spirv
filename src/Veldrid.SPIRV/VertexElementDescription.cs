using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

/// <summary>
/// Describes a single element of a vertex.
/// </summary>
public record struct VertexElementDescription(
    string Name,
    VertexElementSemantic Semantic,
    VertexElementFormat Format,
    uint Offset = 0);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeVertexElementDescription
{
    public InteropArray<byte> Name;
    public VertexElementSemantic Semantic;
    public VertexElementFormat Format;
    public uint Offset;

    public VertexElementDescription ToManaged() =>
        new(Util.GetString(Name.AsSpan()),
            Semantic,
            Format,
            Offset);
}
