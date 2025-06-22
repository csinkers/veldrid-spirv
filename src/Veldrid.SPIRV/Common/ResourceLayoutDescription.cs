// ReSharper disable once CheckNamespace

using System;

namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// Describes the layout of resources in a pipeline's resource set.
/// </summary>
public record struct ResourceLayoutDescription(params ResourceLayoutElementDescription[] Elements)
{
    /// <summary>
    /// Element-wise equality.
    /// </summary>
    /// <param name="other">The instance to compare to.</param>
    /// <returns>True if all array elements are equal; false otherwise.</returns>
    public bool Equals(ResourceLayoutDescription other) => Elements.AsSpan().SequenceEqual(other.Elements);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode() => HashHelper.Array(Elements);
};