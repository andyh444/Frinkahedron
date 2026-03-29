using Veldrid;
using Veldrid.ImageSharp;

namespace Frinkahedron.VeldridImplementation
{
    public class TextureInfo : IDisposable
    {
        public required Texture Texture { get; init; }
        public required TextureView TextureView { get; init; }
        public required Sampler Sampler { get; init; }
        public required ResourceSet ResourceSet { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }

        public static TextureInfo Create(ResourceFactory factory, GraphicsDevice graphicsDevice, TextureDescription textureDescription)
        {
            Texture texture = factory.CreateTexture(textureDescription);
            TextureView textureView = factory.CreateTextureView(texture);
            Sampler sampler = graphicsDevice.LinearSampler;
            var textureLayout = GetResourceLayout(factory);

            ResourceSet textureSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    textureLayout,
                    textureView,
                    sampler));

            return new TextureInfo
            {
                Texture = texture,
                TextureView = textureView,
                Sampler = sampler,
                ResourceSet = textureSet,
                ResourceLayout = textureLayout
            };
        }

        public static TextureInfo Create(ResourceFactory factory, GraphicsDevice graphicsDevice, Stream stream, bool srgb)
        {
            var ist = new ImageSharpTexture(stream, true, srgb);
            return GetTextureInfo(factory, graphicsDevice, ist);
        }

        public static TextureInfo Create(ResourceFactory factory, GraphicsDevice graphicsDevice, string filePath, bool srgb)
        {
            var ist = new ImageSharpTexture(filePath, true, srgb);
            return GetTextureInfo(factory, graphicsDevice, ist);
        }

        private static TextureInfo GetTextureInfo(ResourceFactory factory, GraphicsDevice graphicsDevice, ImageSharpTexture ist)
        {
            Texture texture = ist.CreateDeviceTexture(graphicsDevice, factory);
            TextureView textureView = factory.CreateTextureView(texture);
            Sampler sampler = graphicsDevice.Aniso4xSampler;
            var textureLayout = GetResourceLayout(factory);

            ResourceSet textureSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    textureLayout,
                    textureView,
                    sampler));

            return new TextureInfo
            {
                Texture = texture,
                TextureView = textureView,
                Sampler = sampler,
                ResourceSet = textureSet,
                ResourceLayout = textureLayout
            };
        }

        public static ResourceLayout GetResourceLayout(ResourceFactory factory)
        {
            return factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TextureSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
        }

        public void Dispose()
        {
            Texture.Dispose();
            TextureView.Dispose();
            Sampler.Dispose();
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
        }
    }
}
