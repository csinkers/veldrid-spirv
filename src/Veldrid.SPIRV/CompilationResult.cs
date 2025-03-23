using System;
using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

[StructLayout(LayoutKind.Sequential)]
internal struct CompilationResult
{
    public Bool32 Succeeded;
    public InteropArray<InteropArray<byte>> DataBuffers;
    public ReflectionInfo ReflectionInfo;

    public uint Count => DataBuffers.Count;
    public ReadOnlySpan<byte> Data(int index)
    {
        InteropArray<byte> buffer = DataBuffers.AsSpan()[index];
        return buffer.AsSpan();
    }
}
