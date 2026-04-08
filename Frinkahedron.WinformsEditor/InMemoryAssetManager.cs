using Frinkahedron.VeldridImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinformsEditor
{
    public sealed class InMemoryAssetManager : IAssetManager
    {
        private readonly Dictionary<string, TextureInfo> textures;
        private readonly Dictionary<string, byte[]> shaders;
        private readonly Dictionary<string, Model> models;

        public InMemoryAssetManager()
        {
            textures = new Dictionary<string, TextureInfo>();
            shaders = new Dictionary<string, byte[]>();
            models = new Dictionary<string, Model>();
        }

        public void Dispose()
        {
        }

        public void ClearModels() => models.Clear();

        public void AddShadersFromFolder(string folder)
        {
            foreach (string path in Directory.EnumerateFiles(folder))
            {
                string shaderCode = File.ReadAllText(path);
                string key = Path.GetFileName(path);
                byte[] shaderCodeUtf8 = Encoding.UTF8.GetBytes(shaderCode);
                shaders.Add(key, shaderCodeUtf8);
            }
        }

        public bool HasModel(string name) => models.ContainsKey(name);

        public void AddModel(string name, Model model) => models[name] = model;

        public Model GetModel(string name) => models[name];

        public TextureInfo GetTexture(string v)
        {
            return textures[v];
        }

        public byte[] GetShaderCode(string v)
        {
            return shaders[v];
        }
    }
}
