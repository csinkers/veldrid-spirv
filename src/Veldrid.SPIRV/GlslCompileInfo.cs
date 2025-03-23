using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct GlslCompileInfo
{
    /// <summary>
    /// Element type: byte
    /// </summary>
    public InteropArray<byte> SourceText;

    /// <summary>
    /// Element type: byte
    /// </summary>
    public InteropArray<byte> FileName;
    public ShadercShaderKind Kind;
    public Bool32 Debug;

    /// <summary>
    /// Element type: NativeMacroDefinition
    /// </summary>
    public InteropArray<NativeMacroDefinition> Macros;
};
