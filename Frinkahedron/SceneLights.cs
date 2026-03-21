using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class SceneLights
    {
        public List<PointLight> PointLights { get; }

        public DirectionalLight? DirectionalLight { get; set; }

        public SceneLights()
        {
            PointLights = new List<PointLight>();
        }
    }

    public readonly record struct DirectionalLight(Vector3 Direction, Vector3 Colour);

    public readonly record struct PointLight(Vector3 Position, Vector3 Colour, float Range);
}