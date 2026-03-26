using Frinkahedron.Core;
using System.Numerics;

namespace Frinkahedron.TestApp
{
    internal static class SceneExtensions
    {
        public static PointLightsInfo GetPointLights(this Scene scene)
        {
            Vector3 cameraPosition = scene.Camera.Position;
            var lights = scene.SceneLights.PointLights
                .OrderBy(x => Vector3.DistanceSquared(x.Position, cameraPosition))
                .Select(x => new PointLightInfo { Position = x.Position, Colour = x.Colour, Range = x.Range })
                .ToList();

            PointLightsInfo lightsInfo = new PointLightsInfo();
            if (lights.Count > 0)
            {
                lightsInfo.PointLights0 = lights[0];
            }
            if (lights.Count > 1)
            {
                lightsInfo.PointLights1 = lights[1];
            }
            if (lights.Count > 2)
            {
                lightsInfo.PointLights1 = lights[2];
            }
            if (lights.Count > 3)
            {
                lightsInfo.PointLights1 = lights[3];
            }
            //lightsInfo.NumActiveLights = Math.Min(4, lights.Count);
            return lightsInfo;
        }

        public static DirectionalLightInfo GetDirectionalLight(this Scene scene)
        {
            var directionalLight = scene.SceneLights.DirectionalLight;
            if (directionalLight is null)
            {
                return new DirectionalLightInfo { Enabled = 0 };
            }
            return new DirectionalLightInfo
            {
                Direction = directionalLight.Value.Direction,
                Colour = directionalLight.Value.Colour,
                Enabled = 1
            };
        }

        public static CameraInfo GetCameraInfo(this Scene scene)
        {
            return new CameraInfo
            {
                WorldPosition = scene.Camera.Position,
                LookDirection = scene.Camera.LookDirection,
            };
        }

        public static Camera GetDirectionalLightCamera(this DirectionalLight light)
        {
            return Camera.CreateOrthoCamera(-light.Direction * 150, light.Direction);
        }
    }
}
