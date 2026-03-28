using System.Text;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class AssetManager : IDisposable
    {
        private readonly IReadOnlyDictionary<string, TextureInfo> textures;
        private readonly IReadOnlyDictionary<string, byte[]> shaders;

        private AssetManager(IReadOnlyDictionary<string, TextureInfo> textures, IReadOnlyDictionary<string, byte[]> shaders)
        {
            this.textures = textures;
            this.shaders = shaders;
        }

        public static AssetManager LoadAssets(ResourceFactory factory, GraphicsDevice graphicsDevice, string assetsFolder)
        {
            Dictionary<string, TextureInfo> textures = new Dictionary<string, TextureInfo>();
            foreach (string path in Directory.EnumerateFiles(Path.Combine(assetsFolder, "Textures"), "*.png"))
            {
                string key = Path.GetFileNameWithoutExtension(path);
                textures.Add(key, TextureInfo.Create(factory, graphicsDevice, path));
            }

            Dictionary<string, byte[]> shaders = new Dictionary<string, byte[]>();
            foreach (string path in Directory.EnumerateFiles(Path.Combine(assetsFolder, "Shaders")))
            {
                string shaderCode = File.ReadAllText(path);
                string key = Path.GetFileName(path);
                byte[] shaderCodeUtf8 = Encoding.UTF8.GetBytes(shaderCode);
                shaders.Add(key, shaderCodeUtf8);
            }
            return new AssetManager(textures, shaders);
        }

        internal ResourceSet GetTextureResourceSet(string v)
        {
            return textures[v].ResourceSet;
        }

        internal byte[] GetShaderCode(string v)
        {
            return shaders[v];
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
