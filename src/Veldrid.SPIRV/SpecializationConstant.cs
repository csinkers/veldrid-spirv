using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal record struct NativeSpecializationConstant(uint Id, ulong Data);

/// <summary>
///
/// </summary>
public record struct SpecializationConstant(uint Id, ShaderConstantType Type, ulong Data)
{
    internal NativeSpecializationConstant ToNative() => new(Id, Data);

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a boolean.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, bool value) : this(id, ShaderConstantType.Bool, Store(value ? (byte)1u : (byte)0u)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 16-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, ushort value) : this(id, ShaderConstantType.UInt16, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 16-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, short value) : this(id, ShaderConstantType.Int16, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, uint value) : this(id, ShaderConstantType.UInt32, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, int value) : this(id, ShaderConstantType.Int32, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit signed integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, long value) : this(id, ShaderConstantType.Int64, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit unsigned integer.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, ulong value) : this(id, ShaderConstantType.UInt64, value) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 32-bit floating-point value.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, float value) : this(id, ShaderConstantType.Float, Store(value)) { }

    /// <summary>
    /// Constructs a new <see cref="SpecializationConstant"/> for a 64-bit floating-point value.
    /// </summary>
    /// <param name="id">The constant variable ID.</param>
    /// <param name="value">The constant value.</param>
    public SpecializationConstant(uint id, double value) : this(id, ShaderConstantType.Double, Store(value)) { }

    static unsafe ulong Store<T>(T value)
    {
        ulong ret;
        Unsafe.Write(&ret, value);
        return ret;
    }
}

