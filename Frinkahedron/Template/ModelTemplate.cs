namespace Frinkahedron.Core.Template
{
    public sealed class ModelTemplate(string modelID, string modelPath)
    {
        public string ModelID { get; set; } = modelID;

        public string ModelPath { get; set; } = modelPath;
    }
}
