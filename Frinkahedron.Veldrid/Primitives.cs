using System.Numerics;
using Veldrid;
using Frinkahedron.Core.Meshes;
using Frinkahedron.Core;
using SharpGLTF.Schema2;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class Primitives : IDisposable
    {
        public required IReadOnlyDictionary<Primitive, MeshInfo<TexVertex3>> MeshPrimitives { get; init; }
        public required IReadOnlyDictionary<Primitive, WireframeInfo> WireframePrimitives { get; init; }

        public static Primitives Create(GraphicsDevice graphicsDevice)
        {
            var cubeInfo = MeshInfo.Create(TexMeshFactory.CreateUnitCubeMesh(), graphicsDevice);
            var sphereInfo = MeshInfo.Create(TexMeshFactory.CreateUnitUVSphere(24, 24, RgbaFloat.Red.ToVector4(), RgbaFloat.Blue.ToVector4()), graphicsDevice);
            var cylinderInfo = MeshInfo.Create(TexMeshFactory.CreateUnitCylinderMesh(24, new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4(), new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4()), graphicsDevice);
            var discInfo = MeshInfo.Create(TexMeshFactory.CreateUnitDiscMesh(24), graphicsDevice);

            

            var cubeWireInfo = WireframeInfo.Create(WireframeMeshFactory.CreateUnitWireframeCube(RgbaFloat.White.ToVector4()), graphicsDevice);
            var sphereWireInfo = WireframeInfo.Create(
                WireframeMeshFactory.Combine(
                    [
                        WireframeMeshFactory.CreateUnitWireframeRing(RgbaFloat.White.ToVector4(), Vector3.UnitX),
                        WireframeMeshFactory.CreateUnitWireframeRing(RgbaFloat.White.ToVector4(), Vector3.UnitY),
                        WireframeMeshFactory.CreateUnitWireframeRing(RgbaFloat.White.ToVector4(), Vector3.UnitZ),
                    ]),
                graphicsDevice);

            return new Primitives
            {
                MeshPrimitives = new Dictionary<Primitive, MeshInfo<TexVertex3>>
                {
                    { Primitive.Box, cubeInfo },
                    { Primitive.Ellipsoid, sphereInfo },
                    { Primitive.Cylinder, cylinderInfo },
                    { Primitive.Disc, discInfo },
                },
                WireframePrimitives = new Dictionary<Primitive, WireframeInfo>
                {
                    { Primitive.Box, cubeWireInfo },
                    { Primitive.Ellipsoid, sphereWireInfo},
                }
            };
        }

        public void Dispose()
        {
            foreach (var item in MeshPrimitives)
            {
                item.Value.Dispose();
            }
            foreach (var item in WireframePrimitives)
            {
                item.Value.Dispose();
            }
        }
    }
}
