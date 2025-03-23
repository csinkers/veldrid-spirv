using System;

namespace Veldrid.SPIRV;

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
