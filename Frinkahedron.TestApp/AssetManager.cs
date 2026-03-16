using Veldrid;

namespace Frinkahedron.TestApp
{
    internal sealed class AssetManager : IDisposable
    {
        private readonly IReadOnlyDictionary<string, TextureInfo> textures;

        private AssetManager(IReadOnlyDictionary<string, TextureInfo> textures)
        {
            this.textures = textures;
        }

        public static AssetManager LoadAssets(ResourceFactory factory, GraphicsDevice graphicsDevice, string imageFolder)
        {
            Dictionary<string, TextureInfo> textures = new Dictionary<string, TextureInfo>();
            foreach (string path in Directory.EnumerateFiles(imageFolder, "*.png"))
            {
                string key = Path.GetFileNameWithoutExtension(path);
                textures.Add(key, TextureInfo.Create(factory, graphicsDevice, path));
            }
            return new AssetManager(textures);
        }

        internal ResourceSet GetTextureResourceSet(string v)
        {
            return textures[v].ResourceSet;
        }

        public void Dispose()
        {
            foreach (var tex in textures.Values)
            {
                tex.Dispose();
            }
        }
    }
}
