namespace Veldrid.SPIRV;

/// <summary>
/// An object used to control the parameters of shader translation from SPIR-V to some target language.
/// </summary>
public class CrossCompileOptions
{
    internal static CrossCompileOptions Default { get; } = new();

    /// <summary>
    /// Indicates whether the compiled shader output should include a clip-space Z-range fixup at the end of the
    /// vertex shader.
    /// If true, then the shader will include code that assumes the clip space needs to be corrected from the
    /// "wrong" range into the "right" range for the particular type of shader. For example, if an OpenGL shader is being
    /// generated, then the vertex shader will include a fixup that converts the depth range from [0, 1] to [-1, 1].
    /// If a Direct3D shader is being generated, then the vertex shader will include a fixup that converts the depth range
    /// from [-1, 1] to [0, 1].
    /// </summary>
    public bool FixClipSpaceZ { get; set; }

    /// <summary>
    /// Indicates whether the compiled shader output should include a fixup at the end of the vertex shader which
    /// inverts the clip-space Y value.
    /// </summary>
    public bool InvertVertexOutputY { get; set; }

    /// <summary>
    /// Indicates whether all resource names should be forced into a normalized form. This has functional impact
    /// on compilation targets where resource names are meaningful, like GLSL.
    /// </summary>
    public bool NormalizeResourceNames { get; set; }

    /// <summary>
    /// An array of <see cref="SpecializationConstant"/> which will be substituted into the shader as new constants. Each
    /// element in the array will be matched by ID with the SPIR-V specialization constants defined in the shader.
    /// </summary>
    public SpecializationConstant[] Specializations { get; set; } = [];
}
