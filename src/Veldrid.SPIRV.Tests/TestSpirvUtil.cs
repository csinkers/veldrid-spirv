using System;
using System.IO;

namespace Veldrid.SPIRV.Tests;

internal static class TestSpirvUtil
{
    public static string LoadShaderText(string name)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestShaders", name));

    public static byte[] LoadBytes(string name)
        => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "TestShaders", name));
}
