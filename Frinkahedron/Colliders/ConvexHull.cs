using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public sealed class ConvexHull(BasicMesh mesh) : IShape
    {
        public BasicMesh Mesh { get; } = mesh;

        public AxisAlignedBoundingBox CalculateAABB(Position position)
        {
            // TODO: Test this out by transforming the mesh vertices to world space and using Project with just Vector3.UnitX, etc to confirm we get the same results

            var inverseRot = Quaternion.Conjugate(position.Orientation);

            var axisX = Vector3.Transform(Vector3.UnitX, inverseRot);
            var axisY = Vector3.Transform(Vector3.UnitY, inverseRot);
            var axisZ = Vector3.Transform(Vector3.UnitZ, inverseRot);

            Project(axisX, out var minX, out var maxX);
            Project(axisY, out var minY, out var maxY);
            Project(axisZ, out var minZ, out var maxZ);

            return new AxisAlignedBoundingBox(
               position.Centre + new Vector3(minX, minY, minZ),
               position.Centre + new Vector3(maxX, maxY, maxZ));
        }

        private void Project(
            Vector3 axis,
            out float min,
            out float max)
        {
            min = Vector3.Dot(Mesh.Vertices[0], axis);
            max = min;

            for (int i = 1; i < Mesh.Vertices.Length; i++)
            {
                float p = Vector3.Dot(Mesh.Vertices[i], axis);
                if (p < min)
                {
                    min = p;
                }
                if (p > max)
                {
                    max = p;
                }
            }
        }

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass)
        {
            throw new NotImplementedException();
        }

        public float CalculateVolume()
        {
            throw new NotImplementedException();
        }

        public void Draw(IRenderer renderer, Matrix4x4 position)
        {
            throw new NotImplementedException();
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result)
        {
            result = Vector3.Zero;
            return false;
        }
    }
}
