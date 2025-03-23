namespace Veldrid.SPIRV;

/// <summary>
/// Valid types for a resource in a resource-set.
/// </summary>
public enum ResourceKind : byte
{
    /// <summary>
    /// A uniform buffer.
    /// </summary>
    UniformBuffer,

    /// <summary>
    /// A read-only storage buffer.
    /// </summary>
    StructuredBufferReadOnly,

    /// <summary>
    /// A read-write storage buffer.
    /// </summary>
    StructuredBufferReadWrite,

    /// <summary>
    /// A read-only texture.
    /// </summary>
    TextureReadOnly,

    /// <summary>
    /// A read-write texture.
    /// </summary>
    TextureReadWrite,

    /// <summary>
    /// A texture sampler.
    /// </summary>
    Sampler,
}
