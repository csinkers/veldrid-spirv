using System;
using System.Text;

namespace Veldrid.SPIRV;

/// <summary>
/// Utility methods for SPIR-V shaders.
/// </summary>
public static class SpirvUtil
{
    static Encoding Utf8 { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Converts the given byte array to a UTF-8 string.
    /// </summary>
    public static string GetString(ReadOnlySpan<byte> data) => Utf8.GetString(data);

    /// <summary>
    /// Checks if the given byte array has a SPIR-V header.
    /// </summary>
    public static bool HasSpirvHeader(byte[] bytes) => bytes is [0x03, 0x02, 0x23, 0x07, ..];

    internal static bool ArrayEqualsEquatable<T>(T[]? left, T[]? right)
        where T : struct, IEquatable<T>
        => left.AsSpan().SequenceEqual(right.AsSpan());
}
