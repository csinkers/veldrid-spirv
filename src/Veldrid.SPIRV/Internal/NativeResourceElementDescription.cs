using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceElementDescription
{
    public InteropArray<byte> Name;
    public ResourceKind Kind;
    public ShaderStages Stages;
    public ResourceLayoutElementOptions Options;

    public ResourceLayoutElementDescription ToManaged() =>
        new(SpirvUtil.GetString(Name.AsSpan()),
            Kind,
            Stages,
            Options);
}