using Frinkahedron.Core.Maths;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct CylinderBoxTester : ICollisionPairTester<Cylinder, Box>
    {
        public static CollisionManifold Test(Collidable<Cylinder> shapeA, Collidable<Box> shapeB)
        {
            Vector3 cylAxis = Vector3.Transform(Vector3.UnitY, shapeA.Position.Orientation);

            Vector3 circleCentre1 = shapeA.Position.Centre - cylAxis * shapeA.Shape.Height / 2;
            Vector3 circleCentre2 = shapeA.Position.Centre + cylAxis * shapeA.Shape.Height / 2;

            LineSegment cylinderSeg = new LineSegment(circleCentre1, circleCentre2);

            Vector3 boxPlane1Point = Vector3.Transform(shapeB.Shape.Dimensions / 2, shapeB.Position.ToMatrix());
            Vector3 boxPlane1Normal = Vector3.Transform(Vector3.UnitY, shapeB.Position.Orientation);

            List<Vector3> points = new List<Vector3>();

            // TODO Do all faces of the box and consider cases where the cylinder touches a box corner or edge
            var plane = new Maths.Plane(boxPlane1Point, boxPlane1Normal);
            float bestOverlap = 0;

            {
                float overlap = plane.CirclePlanePenetration(circleCentre1, cylAxis, shapeA.Shape.Radius, out var penetrationPoint);
                if (overlap > 0)
                {
                    bestOverlap = MathF.Max(-plane.DistanceTo(penetrationPoint), bestOverlap);
                    points.Add(penetrationPoint);
                }
            }
            {
                float overlap = plane.CirclePlanePenetration(circleCentre2, cylAxis, shapeA.Shape.Radius, out var penetrationPoint);
                if (overlap > 0)
                {
                    bestOverlap = MathF.Max(-plane.DistanceTo(penetrationPoint), bestOverlap);
                    points.Add(penetrationPoint);
                }
            }

            if (points.Any())
            {
                return new CollisionManifold(points.ToArray(), boxPlane1Normal, bestOverlap);
            }
            return CollisionManifold.NoCollision();
        }

    }
}
