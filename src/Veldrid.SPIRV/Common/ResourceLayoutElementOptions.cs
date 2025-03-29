using System;

// ReSharper disable once CheckNamespace
namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Miscellaneous options for an element in a ResourceLayout.
/// </summary>
[Flags]
public enum ResourceLayoutElementOptions
{
    /// <summary>
    /// No special options.
    /// </summary>
    None,

    /// <summary>
    /// Can be applied to a buffer type resource allowing it to be bound with a dynamic offset.
    /// </summary>
    DynamicBinding = 1 << 0,
}
