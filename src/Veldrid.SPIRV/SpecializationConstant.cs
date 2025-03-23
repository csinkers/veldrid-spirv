using System.Runtime.CompilerServices;

namespace Veldrid.SPIRV;

/// <summary>
///
/// </summary>
public record struct SpecializationConstant(uint Id, SpecializationConstantType Type, ulong Data)
{
    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a boolean.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, bool value) : this(id, SpecializationConstantType.Bool, Store(value ? (byte)1u : (byte)0u)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 16-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, ushort value) : this(id, SpecializationConstantType.UInt16, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 16-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, short value) : this(id, SpecializationConstantType.Int16, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, uint value) : this(id, SpecializationConstantType.UInt32, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, int value) : this(id, SpecializationConstantType.Int32, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, ulong value) : this(id, SpecializationConstantType.UInt64, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, long value) : this(id, SpecializationConstantType.Int64, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit floating-point value.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, float value) : this(id, SpecializationConstantType.Float, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit floating-point value.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, double value) : this(id, SpecializationConstantType.Double, Store(value)) { }

    static unsafe ulong Store<T>(T value)
    {
        ulong ret;
        Unsafe.Write(&ret, value);
        return ret;
    }
}

