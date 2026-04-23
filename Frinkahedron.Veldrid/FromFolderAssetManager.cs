using System.Text;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class FromFolderAssetManager : IAssetManager
    {
        private readonly IReadOnlyDictionary<string, TextureInfo> textures;
        private readonly IReadOnlyDictionary<string, byte[]> shaders;
        private readonly IReadOnlyDictionary<string, Model> models;

        private FromFolderAssetManager(IReadOnlyDictionary<string, TextureInfo> textures, IReadOnlyDictionary<string, byte[]> shaders, IReadOnlyDictionary<string, Model> models)
        {
            this.textures = textures;
            this.shaders = shaders;
            this.models = models;
        }

        public static FromFolderAssetManager LoadAssets(ResourceFactory factory, GraphicsDevice graphicsDevice, string assetsFolder)
        {
            Dictionary<string, TextureInfo> textures = new Dictionary<string, TextureInfo>();
            foreach (string path in Directory.EnumerateFiles(Path.Combine(assetsFolder, "Textures"), "*.png"))
            {
                string key = Path.GetFileNameWithoutExtension(path);
                textures.Add(key, TextureInfo.Create(factory, graphicsDevice, path, !key.Contains("Map")));
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
            models.Add("bowling_ball", ModelLoader.LoadModel(factory, graphicsDevice, @"D:\3D Models\bowling_ball\scene.gltf", textures["white"]));
            models.Add("simple_classic_crate", ModelLoader.LoadModel(factory, graphicsDevice, @"D:\3D Models\simple_classic_crate\scene.gltf", textures["white"]));
            models.Add("tin_can_damaged", ModelLoader.LoadModel(factory, graphicsDevice, @"D:\3D Models\tin_can_damaged\scene.gltf", textures["white"]));
            models.Add("old_rusty_car", ModelLoader.LoadModel(factory, graphicsDevice, @"D:\3D Models\old_rusty_car\scene.gltf", textures["white"]));
            return new FromFolderAssetManager(textures, shaders, models);
        }

        public Model GetModel(string name) => models[name];

        public TextureInfo GetTexture(string v)
        {
            return textures[v];
        }

        public byte[] GetShaderCode(string v)
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
