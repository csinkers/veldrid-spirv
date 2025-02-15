using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Veldrid.SPIRV;

/// <summary>
/// Contains information about the vertex attributes and resource types, and their binding slots, for a compiled
/// set of shaders. This information can be used to construct <see cref="ResourceLayout"/> and
/// <see cref="Pipeline"/> objects.
/// </summary>
public class SpirvReflection
{
    /// <summary>
    /// An array containing a description of each vertex element that is used by the compiled shader set.
    /// This array will be empty for compute shaders.
    /// </summary>
    public VertexElementDescription[] VertexElements { get; }

    /// <summary>
    /// An array containing a description of each set of resources used by the compiled shader set.
    /// </summary>
    public ResourceLayoutDescription[] ResourceLayouts { get; }

    /// <summary>
    /// Constructs a new <see cref="SpirvReflection"/> instance.
    /// </summary>
    /// <param name="vertexElements">/// An array containing a description of each vertex element that is used by
    /// the compiled shader set.</param>
    /// <param name="resourceLayouts">An array containing a description of each set of resources used by the
    /// compiled shader set.</param>
    public SpirvReflection(
        VertexElementDescription[] vertexElements,
        ResourceLayoutDescription[] resourceLayouts
    )
    {
        VertexElements = vertexElements;
        ResourceLayouts = resourceLayouts;
    }

    /// <summary>
    /// Loads a <see cref="SpirvReflection"/> object from a serialized JSON file at the given path.
    /// </summary>
    /// <param name="jsonPath">The path to the JSON file.</param>
    /// <returns>A new <see cref="SpirvReflection"/> object, deserialized from the file.</returns>
    public static SpirvReflection? LoadFromJson(string jsonPath)
    {
        using FileStream jsonStream = File.OpenRead(jsonPath);
        return LoadFromJson(jsonStream);
    }

    /// <summary>
    /// Loads a <see cref="SpirvReflection"/> object from a serialized JSON stream.
    /// </summary>
    /// <param name="jsonStream">The stream of serialized JSON text.</param>
    /// <returns>A new <see cref="SpirvReflection"/> object, deserialized from the stream.</returns>
    public static SpirvReflection? LoadFromJson(Stream jsonStream)
    {
        return JsonSerializer.Deserialize(
            jsonStream,
            SpirvReflectionJsonContext.Default.SpirvReflection
        );
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    UseStringEnumConverter = true,
    IncludeFields = true
)]
[JsonSerializable(typeof(SpirvReflection))]
internal partial class SpirvReflectionJsonContext : JsonSerializerContext { }
