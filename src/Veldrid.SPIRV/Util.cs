using System;
using System.Reflection;
using System.Text;

namespace Veldrid.SPIRV;

internal static class Util
{
    static Encoding Utf8 { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    internal static string GetString(ReadOnlySpan<byte> data) => Utf8.GetString(data);
    internal static bool HasSpirvHeader(byte[] bytes) => bytes is [0x03, 0x02, 0x23, 0x07, ..];
    internal static bool ArrayEqualsEquatable<T>(T[]? left, T[]? right)
        where T : struct, IEquatable<T>
        => left.AsSpan().SequenceEqual(right.AsSpan());
}
