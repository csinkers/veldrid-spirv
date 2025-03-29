// ReSharper disable once CheckNamespace
namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Describes the layout of resources in a pipeline's resource set.
/// </summary>
public record struct ResourceLayoutDescription(ResourceLayoutElementDescription[] Elements);