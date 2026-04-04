namespace Frinkahedron.VeldridImplementation
{
    public interface IAssetManager : IDisposable
    {
        Model GetModel(string name);

        TextureInfo GetTexture(string v);

        byte[] GetShaderCode(string v);
    }
}