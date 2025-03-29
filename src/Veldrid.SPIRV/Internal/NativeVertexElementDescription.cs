using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeVertexElementDescription
{
    public InteropArray<byte> Name;
    public VertexElementSemantic Semantic;
    public VertexElementFormat Format;
    public uint Offset;

    public VertexElementDescription ToManaged() =>
        new(SpirvUtil.GetString(Name.AsSpan()),
            Semantic,
            Format,
            Offset);
}