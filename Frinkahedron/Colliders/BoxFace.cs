using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public readonly record struct BoxFace(Vector3 Centre, Vector3 Tangent1, Vector3 Tangent2, Vector3 Normal, float HalfExtent1, float HalfExtent2)
    {
        public static BoxFace GetFace(
            Vector3 boxCenter,
            Span<Vector3> axes,
            Vector3 halfDimensions,
            Vector3 normal)
        {
            Vector3 axisX = axes[0];
            Vector3 axisY = axes[1];
            Vector3 axisZ = axes[2];

            var nax = Vector3.Dot(normal, axisX);
            var nay = Vector3.Dot(normal, axisY);
            var naz = Vector3.Dot(normal, axisZ);

            float dx = MathF.Abs(nax);
            float dy = MathF.Abs(nay);
            float dz = MathF.Abs(naz);

            if (dx > dy && dx > dz)
            {
                var faceNormal = nax > 0 ? axisX : -axisX;

                return new BoxFace(
                    boxCenter + faceNormal * halfDimensions.X,
                    axisY,
                    axisZ,
                    faceNormal,
                    halfDimensions.Y,
                    halfDimensions.Z);
            }

            if (dy > dz)
            {
                var faceNormal = nay > 0 ? axisY : -axisY;

                return new BoxFace(
                    boxCenter + faceNormal * halfDimensions.Y,
                    axisX,
                    axisZ,
                    faceNormal,
                    halfDimensions.X,
                    halfDimensions.Z);
            }

            var faceNormalZ = naz > 0 ? axisZ : -axisZ;

            return new BoxFace(
                boxCenter + faceNormalZ * halfDimensions.Z,
                axisX,
                axisY,
                faceNormalZ,
                halfDimensions.X,
                halfDimensions.Y);
        }

        public List<Vector3> GetVertices()
        {
            var t1 = Tangent1 * HalfExtent1;
            var t2 = Tangent2 * HalfExtent2;

            return [
                Centre - t1 - t2,
                    Centre + t1 - t2,
                    Centre + t1 + t2,
                    Centre - t1 + t2
            ];
        }
    }

}
