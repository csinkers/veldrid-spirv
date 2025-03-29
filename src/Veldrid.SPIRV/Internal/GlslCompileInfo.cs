using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct GlslCompileInfo
{
    public InteropArray<byte> SourceText;
    public InteropArray<byte> FileName;
    public ShadercShaderKind Kind;
    public Bool32 Debug;
    public InteropArray<NativeMacroDefinition> Macros;
};
