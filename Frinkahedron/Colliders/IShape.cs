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
    public interface IShape
    {
        public void Draw(IRenderContext renderer, Matrix4x4 position);

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass);

        public float CalculateVolume();

        public AxisAlignedBoundingBox CalculateAABB(Position position);

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result, out Vector3 normal);
    }
}
