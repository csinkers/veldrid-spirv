using Xunit;

namespace Veldrid.SPIRV.Tests
{
    public class EqualityAndHashTests
    {
        [Fact]
        public void TestResourceLayoutDescriptionHashing()
        {
            ResourceLayoutDescription layout1 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );

            ResourceLayoutDescription layout2 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );

            ResourceLayoutDescription layout3 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test2", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );

            Assert.Equal(layout1.GetHashCode(), layout2.GetHashCode());
            Assert.NotEqual(layout1.GetHashCode(), layout3.GetHashCode());
        }

        [Fact]
        public void TestResourceLayoutDescriptionEquality()
        {
            ResourceLayoutDescription layout1 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );
            ResourceLayoutDescription layout2 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );
            ResourceLayoutDescription layout3 = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Test2", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
            );

            Assert.Equal(layout1, layout2);
            Assert.NotEqual(layout1, layout3);
        }
    }
}
