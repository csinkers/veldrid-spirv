namespace Veldrid.SPIRV;

/// <summary>
/// Describes the layout of resources in a pipeline's resource set.
/// </summary>
public record struct ResourceLayoutDescription(ResourceLayoutElementDescription[] Elements);
