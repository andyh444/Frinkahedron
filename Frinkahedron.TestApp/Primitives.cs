using System.Numerics;
using Veldrid;
using Frinkahedron.Core;

namespace Frinkahedron.TestApp
{
    internal sealed class Primitives : IDisposable
    {
        public required MeshInfo CubeInfo { get; init; }
        public required MeshInfo SphereInfo { get; init; }
        public required MeshInfo CylinderInfo { get; init; }

        public static Primitives Create(GraphicsDevice graphicsDevice)
        {
            Mesh mesh = CreateUnitCubeMesh();
            var cubeInfo = MeshInfo.Create(mesh, graphicsDevice);
            var sphereInfo = MeshInfo.Create(CreateUnitUVSphere(24, 24, RgbaFloat.Red.ToVector4(), RgbaFloat.Blue.ToVector4()), graphicsDevice);
            var cylinderInfo = MeshInfo.Create(CreateUnitCylinderMesh(24, new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4(), new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4()), graphicsDevice);

            return new Primitives
            {
                CubeInfo = cubeInfo,
                SphereInfo = sphereInfo,
                CylinderInfo = cylinderInfo,
            };
        }

        public void Dispose()
        {
            CubeInfo.Dispose();
            SphereInfo.Dispose();
            CylinderInfo.Dispose();
        }

        private static Mesh CreateQuadMesh()
        {
            VertexPositionColor[] quadVertices =
            {
            new VertexPositionColor(new Vector3(-0.75f, 0.75f, 0), RgbaFloat.Red.ToVector4()),
            new VertexPositionColor(new Vector3(0.75f, 0.75f, 0), RgbaFloat.Green.ToVector4()),
            new VertexPositionColor(new Vector3(-0.75f, -0.75f, 0), RgbaFloat.Blue.ToVector4()),
            new VertexPositionColor(new Vector3(0.75f, -0.75f, 0), RgbaFloat.Yellow.ToVector4())
        };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            return new Mesh(quadVertices, quadIndices);
        }

        private static Mesh CreateUnitCubeMesh()
        {
            VertexPositionColor[] vertices =
            {
               new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), RgbaFloat.Red.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), RgbaFloat.Green.ToVector4()),
               new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), RgbaFloat.Blue.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), RgbaFloat.Yellow.ToVector4()),

               new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), RgbaFloat.Green.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), RgbaFloat.Blue.ToVector4()),
               new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), RgbaFloat.Yellow.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), RgbaFloat.Red.ToVector4())
           };

            ushort[] indices =
            {  
               // Front face  
               0, 1, 2,
               2, 1, 3,  

               // Back face  
               4, 6, 5,
               5, 6, 7,  

               // Left face  
               4, 0, 6,
               6, 0, 2,  

               // Right face  
               1, 5, 3,
               3, 5, 7,  

               // Top face  
               4, 5, 0,
               0, 5, 1,  

               // Bottom face  
               2, 3, 6,
               6, 3, 7
           };

            return new Mesh(vertices, indices);
        }

        public static Mesh CreateUnitCylinderMesh(int segments, Vector4 topColour, Vector4 bottomColour)
        {
            var vertList = new List<VertexPositionColor>();
            var indexList = new List<ushort>();

            float halfHeight = 0.5f;
            float radius = 0.5f;

            // top circle
            for (int i = 0; i < segments; i++)
            {
                float theta = i * 2 * MathF.PI / segments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                Vector3 position = new Vector3(
                    radius * sinTheta,
                    halfHeight,
                    radius * cosTheta);

                vertList.Add(new VertexPositionColor(position, topColour));
            }

            // bottom circle
            for (int i = 0; i < segments; i++)
            {
                float theta = i * 2 * MathF.PI / segments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                Vector3 position = new Vector3(
                    radius * sinTheta,
                    -halfHeight,
                    radius * cosTheta);

                vertList.Add(new VertexPositionColor(position, bottomColour));
            }

            int bottomStart = segments;
            for (int i = 0; i < segments; i++)
            {
                ushort topCurrent = (ushort)i;
                ushort topNext = (ushort)((i + 1) % segments);

                ushort bottomCurrent = (ushort)(topCurrent + bottomStart);
                ushort bottomNext = (ushort)(topNext + bottomStart);

                indexList.Add(topCurrent);
                indexList.Add(topNext);
                indexList.Add(bottomCurrent);

                indexList.Add(topNext);
                indexList.Add(bottomNext);
                indexList.Add(bottomCurrent);
            }

            return new Mesh(vertList.ToArray(), indexList.ToArray());
        }

        /// <summary>
        /// Generates a UV sphere mesh.
        /// </summary>
        /// <param name="radius">Sphere radius</param>
        /// <param name="longitudeSegments">Number of longitudinal slices</param>
        /// <param name="latitudeSegments">Number of latitudinal slices</param>
        /// <param name="color">Vertex color</param>
        /// <param name="vertices">Output vertex array</param>
        /// <param name="indices">Output triangle indices</param>
        public static Mesh CreateUnitUVSphere(int longitudeSegments, int latitudeSegments, Vector4 topColor, Vector4 bottomColour)
        {
            var vertList = new List<VertexPositionColor>();
            var indexList = new List<ushort>();
            float radius = 0.5f;
            // Generate vertices
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * MathF.PI / latitudeSegments; // 0 to π
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                Vector4 colour = Vector4.Lerp(topColor, bottomColour, (float)lat / latitudeSegments);
                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2f * MathF.PI / longitudeSegments; // 0 to 2π
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    Vector3 pos = new Vector3(
                        radius * sinTheta * cosPhi,
                        radius * cosTheta,
                        radius * sinTheta * sinPhi
                    );

                    vertList.Add(new VertexPositionColor(pos, colour));
                }
            }

            // Generate indices
            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    ushort first = (ushort)((lat * (longitudeSegments + 1)) + lon);
                    ushort second = (ushort)(first + longitudeSegments + 1);

                    // Each quad is split into two triangles
                    indexList.Add(first);
                    indexList.Add(second);
                    indexList.Add((ushort)(first + 1));

                    indexList.Add(second);
                    indexList.Add((ushort)(second + 1));
                    indexList.Add((ushort)(first + 1));
                }
            }
            return new Mesh(vertList.ToArray(), indexList.ToArray());
        }

        
    }
}
