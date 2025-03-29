using System.Runtime.InteropServices;

namespace Veldrid.SPIRV.Internal;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct CrossCompileInfo
{
    public CrossCompileTarget Target;
    public Bool32 FixClipSpaceZ;
    public Bool32 InvertY;
    public Bool32 NormalizeResourceNames;
    public InteropArray<NativeSpecializationConstant> Specializations;
    public InteropArray<byte> VertexShader;
    public InteropArray<byte> FragmentShader;
    public InteropArray<byte> ComputeShader;
}
