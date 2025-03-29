using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

internal static unsafe class VeldridSpirvNative
{
    const string LibName = "libveldrid-spirv";

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern CompilationResult* CrossCompile(CrossCompileInfo* info);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern CompilationResult* CompileGlslToSpirv(GlslCompileInfo* info);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreeResult(CompilationResult* result);
}
