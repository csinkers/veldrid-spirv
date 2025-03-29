// ReSharper disable once CheckNamespace
namespace Veldrid; // Put this into the Veldrid namespace to maintain backwards compatibility.

/// <summary>
/// The valid data types for shader constants.
/// </summary>
public enum ShaderConstantType
{
    /// <summary>
    /// A boolean.
    /// </summary>
    Bool,

    /// <summary>
    /// A 16-bit unsigned integer.
    /// </summary>
    UInt16,

    /// <summary>
    /// A 16-bit signed integer.
    /// </summary>
    Int16,

    /// <summary>
    /// A 32-bit unsigned integer.
    /// </summary>
    UInt32,

    /// <summary>
    /// A 32-bit signed integer.
    /// </summary>
    Int32,

    /// <summary>
    /// A 64-bit unsigned integer.
    /// </summary>
    UInt64,

    /// <summary>
    /// A 64-bit signed integer.
    /// </summary>
    Int64,

    /// <summary>
    /// A 32-bit floating-point value.
    /// </summary>
    Float,

    /// <summary>
    /// A 64-bit floating-point value.
    /// </summary>
    Double,
}