using System;
using System.Runtime.InteropServices;

namespace Veldrid.SPIRV;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe struct InteropArray<T>(uint count, void* data) where T : unmanaged
{
    public uint Count = count;
    public void* Data = data;

    public ReadOnlySpan<T> AsSpan() => new((T*)Data, (int)Count);
    public ref T this[uint index] => ref ((T*)Data)[index];
}
