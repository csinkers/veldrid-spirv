using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

/// <summary>
/// Describes the layout of resources in a pipeline's resource set.
/// </summary>
public record struct ResourceLayoutDescription(ResourceLayoutElementDescription[] Elements);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceLayoutDescription
{
    public InteropArray<NativeResourceElementDescription> ResourceElements;
}
