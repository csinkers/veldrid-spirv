namespace Veldrid.SPIRV;

/// <summary>
/// Describes an individual resource element in a ResourceLayout
/// </summary>
public record struct ResourceLayoutElementDescription(
    string Name,
    ResourceKind Kind,
    ShaderStages Stages,
    ResourceLayoutElementOptions Options = ResourceLayoutElementOptions.None);
