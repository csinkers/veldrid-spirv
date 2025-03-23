using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct ReflectionInfo
{
    public InteropArray<NativeVertexElementDescription> VertexElements;
    public InteropArray<NativeResourceLayoutDescription> ResourceLayouts;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeVertexElementDescription
{
    public InteropArray<byte> Name;
    public VertexElementSemantic Semantic;
    public VertexElementFormat Format;
    public uint Offset;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceLayoutDescription
{
    public InteropArray<NativeResourceElementDescription> ResourceElements;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceElementDescription
{
    public InteropArray<byte> Name;
    public ResourceKind Kind;
    public ShaderStages Stages;
    public ResourceLayoutElementOptions Options;
}

