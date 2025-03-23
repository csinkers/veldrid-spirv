using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Veldrid.SPIRV.Tests.DllLoaderHack", "Veldrid.SPIRV.Tests")]
namespace Veldrid.SPIRV.Tests;

/// <summary>
/// Hack to work around ProjectReference not working with native dlls in runtime/.../native dirs
/// </summary>
public sealed class DllLoaderHack : XunitTestFramework
{
    public DllLoaderHack(IMessageSink messageSink)
        : base(messageSink)
    {
        SpirvCompilation.SetImportResolver();
    }
}
