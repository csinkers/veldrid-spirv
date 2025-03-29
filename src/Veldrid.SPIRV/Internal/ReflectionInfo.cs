using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct ReflectionInfo
{
    public InteropArray<NativeVertexElementDescription> VertexElements;
    public InteropArray<NativeResourceLayoutDescription> ResourceLayouts;
}

