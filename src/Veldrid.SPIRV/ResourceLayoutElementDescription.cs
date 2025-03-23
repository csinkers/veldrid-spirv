using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

/// <summary>
/// Describes an individual resource element in a ResourceLayout
/// </summary>
public record struct ResourceLayoutElementDescription(
    string Name,
    ResourceKind Kind,
    ShaderStages Stages,
    ResourceLayoutElementOptions Options = ResourceLayoutElementOptions.None);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NativeResourceElementDescription
{
    public InteropArray<byte> Name;
    public ResourceKind Kind;
    public ShaderStages Stages;
    public ResourceLayoutElementOptions Options;

    public ResourceLayoutElementDescription ToManaged() =>
        new(Util.GetString(Name.AsSpan()),
            Kind,
            Stages,
            Options);
}
