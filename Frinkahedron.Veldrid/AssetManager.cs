using System.Text;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class AssetManager : IDisposable
    {
        private readonly IReadOnlyDictionary<string, TextureInfo> textures;
        private readonly IReadOnlyDictionary<string, byte[]> shaders;
        private readonly IReadOnlyDictionary<string, Model> models;

        private AssetManager(IReadOnlyDictionary<string, TextureInfo> textures, IReadOnlyDictionary<string, byte[]> shaders, IReadOnlyDictionary<string, Model> models)
        {
            this.textures = textures;
            this.shaders = shaders;
            this.models = models;
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

            Dictionary<string, Model> models = new Dictionary<string, Model>();
            models.Add("bowlingball", ModelLoader.LoadModel(factory, graphicsDevice, @"C:\Users\Andy\Downloads\bowling_ball\scene.gltf"));
            return new AssetManager(textures, shaders, models);
        }

        internal Model GetModel(string name) => models[name];

        internal TextureInfo GetTexture(string v)
        {
            return textures[v];
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
