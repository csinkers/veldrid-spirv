using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Veldrid.SPIRV;

internal static class Util
{
    public static Encoding UTF8 { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    [return: NotNullIfNotNull(nameof(data))]
    internal static unsafe string? GetString(byte* data, uint length)
    {
        if (data == null)
        {
            return null!;
        }

        return UTF8.GetString(data, (int)length);
    }

    internal static bool HasSpirvHeader(byte[] bytes)
    {
        return bytes is [0x03, 0x02, 0x23, 0x07, ..];
    }
}
