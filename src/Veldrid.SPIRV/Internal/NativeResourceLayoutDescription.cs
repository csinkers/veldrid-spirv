using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceLayoutDescription
{
    public InteropArray<NativeResourceElementDescription> ResourceElements;
}