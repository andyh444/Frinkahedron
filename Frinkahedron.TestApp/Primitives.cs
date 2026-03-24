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
        public required MeshInfo DiscInfo { get; init; }

        public static Primitives Create(GraphicsDevice graphicsDevice)
        {
            Mesh mesh = CreateUnitCubeMesh();
            var cubeInfo = MeshInfo.Create(mesh, graphicsDevice);
            var sphereInfo = MeshInfo.Create(CreateUnitUVSphere(24, 24, RgbaFloat.Red.ToVector4(), RgbaFloat.Blue.ToVector4()), graphicsDevice);
            var cylinderInfo = MeshInfo.Create(CreateUnitCylinderMesh(24, new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4(), new RgbaFloat(0.5f, 0, 0.5f, 1).ToVector4()), graphicsDevice);
            var discInfo = MeshInfo.Create(CreateUnitDiscMesh(24), graphicsDevice);

            return new Primitives
            {
                CubeInfo = cubeInfo,
                SphereInfo = sphereInfo,
                CylinderInfo = cylinderInfo,
                DiscInfo = discInfo
            };
        }

        public void Dispose()
        {
            CubeInfo.Dispose();
            SphereInfo.Dispose();
            CylinderInfo.Dispose();
            DiscInfo.Dispose();
        }

        /*private static Mesh CreateQuadMesh()
        {
            VertexPositionUv[] quadVertices =
            {
            new VertexPositionUv(new Vector3(-0.75f, 0.75f, 0), RgbaFloat.Red.ToVector4()),
            new VertexPositionUv(new Vector3(0.75f, 0.75f, 0), RgbaFloat.Green.ToVector4()),
            new VertexPositionUv(new Vector3(-0.75f, -0.75f, 0), RgbaFloat.Blue.ToVector4()),
            new VertexPositionUv(new Vector3(0.75f, -0.75f, 0), RgbaFloat.Yellow.ToVector4())
        };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            return new Mesh(quadVertices, quadIndices);
        }*/

        private static Mesh CreateUnitCubeMesh()
        {
            Vertex[] vertices =
            {
                // Front
                new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new Vertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0, 0, 1), new Vector2(1, 1)),

                // Back
                new Vertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 0)),
                new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 0)),
                new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1)),
                new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1)),

                // Left
                new Vertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1, 0, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0, 1)),
                new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1, 0, 0), new Vector2(1, 1)),

                // Right
                new Vertex(new Vector3(0.5f,  0.5f,  0.5f), new Vector3(1, 0, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(0.5f,  0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(0.5f, -0.5f,  0.5f), new Vector3(1, 0, 0), new Vector2(0, 1)),
                new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1, 1)),

                // Top
                new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0, 0)),
                new Vertex(new Vector3( 0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(-0.5f, 0.5f,  0.5f), new Vector3(0, 1, 0), new Vector2(0, 1)),
                new Vertex(new Vector3( 0.5f, 0.5f,  0.5f), new Vector3(0, 1, 0), new Vector2(1, 1)),

                // Bottom
                new Vertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0, -1, 0), new Vector2(0, 0)),
                new Vertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3(0, -1, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(0, 1)),
                new Vertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(1, 1)),
            };

            ushort[] indices =
            {
                0,1,2,  2,1,3,        // Front
                4,5,6,  6,5,7,        // Back
                8,9,10, 10,9,11,      // Left
                12,13,14, 14,13,15,   // Right
                16,17,18, 18,17,19,   // Top
                20,21,22, 22,21,23    // Bottom
            };

            return new Mesh(vertices, indices);
        }

        public static Mesh CreateUnitDiscMesh(int segments)
        {
            var vertList = new List<Vertex>();
            var indexList = new List<ushort>();

            Vector3 normal = Vector3.UnitY;
            Vertex centre = new Vertex(new Vector3(), normal, new Vector2(0.5f, 0.5f));
            vertList.Add(centre);

            float radius = 0.5f;

            for (int i = 0; i < segments; i++)
            {
                float theta = i * 2 * MathF.PI / segments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                Vector3 position = new Vector3(radius * sinTheta, 0, radius * cosTheta);
                vertList.Add(new Vertex(position, normal, new Vector2(0.5f + radius * sinTheta, 0.5f + radius * cosTheta)));
                indexList.Add(0); // centre
                indexList.Add((ushort)(i + 1)); // this one
                indexList.Add((ushort)(((i + 2) % segments) + 1)); // next one
            }

            return new Mesh(vertList.ToArray(), indexList.ToArray());
        }

        public static Mesh CreateUnitCylinderMesh(int segments, Vector4 topColour, Vector4 bottomColour)
        {
            var vertList = new List<Vertex>();
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

                Vector3 normal = new Vector3(sinTheta, 0, cosTheta);

                vertList.Add(new Vertex(position, normal, new Vector2((float)i / segments, 0)));
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

                Vector3 normal = new Vector3(sinTheta, 0, cosTheta);

                vertList.Add(new Vertex(position, normal, new Vector2((float)i / segments, 1)));
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
            var vertList = new List<Vertex>();
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

                    Vector3 normal = new Vector3(sinTheta * cosPhi, cosTheta, sinTheta * sinPhi);
                    Vector3 pos = radius * normal;

                    vertList.Add(new Vertex(pos, normal, new Vector2(phi / (2f * MathF.PI), theta / MathF.PI)));
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
