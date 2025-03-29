// ReSharper disable once CheckNamespace
namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Describes an individual resource element in a ResourceLayout
/// </summary>
public record struct ResourceLayoutElementDescription(
    string Name,
    ResourceKind Kind,
    ShaderStages Stages,
    ResourceLayoutElementOptions Options = ResourceLayoutElementOptions.None);