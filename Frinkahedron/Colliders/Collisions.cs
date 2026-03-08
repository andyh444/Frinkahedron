using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public static class Collisions
    {

        public static CollisionManifold BoxBoxCollision(
            Box box1,
            Position box1Position,
            Box box2,
            Position box2Position)
        {
            Vector3 half1 = box1.Dimensions * 0.5f;
            Vector3 half2 = box2.Dimensions * 0.5f;

            Vector3 c1 = box1Position.Centre;
            Vector3 c2 = box2Position.Centre;

            Quaternion q1 = box1Position.Orientation;
            Quaternion q2 = box2Position.Orientation;

            Matrix4x4 rot1 = Matrix4x4.CreateFromQuaternion(q1);
            Matrix4x4 rot2 = Matrix4x4.CreateFromQuaternion(q2);

            Vector3[] A =
            {
        new Vector3(rot1.M11, rot1.M12, rot1.M13),
        new Vector3(rot1.M21, rot1.M22, rot1.M23),
        new Vector3(rot1.M31, rot1.M32, rot1.M33)
    };

            Vector3[] B =
            {
        new Vector3(rot2.M11, rot2.M12, rot2.M13),
        new Vector3(rot2.M21, rot2.M22, rot2.M23),
        new Vector3(rot2.M31, rot2.M32, rot2.M33)
    };

            float[,] R = new float[3, 3];
            float[,] AbsR = new float[3, 3];

            const float epsilon = 1e-6f;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    R[i, j] = Vector3.Dot(A[i], B[j]);
                    AbsR[i, j] = MathF.Abs(R[i, j]) + epsilon;
                }

            Vector3 tVec = c2 - c1;
            float[] t =
            {
                Vector3.Dot(tVec, A[0]),
                Vector3.Dot(tVec, A[1]),
                Vector3.Dot(tVec, A[2])
            };

            float penetration = float.MaxValue;
            Vector3 bestAxis = default;

            bool TestAxis(Vector3 axis, float overlap)
            {
                if (overlap < 0) return false;

                if (overlap < penetration)
                {
                    penetration = overlap;
                    bestAxis = axis;
                }

                return true;
            }

            // Axes A0..A2
            for (int i = 0; i < 3; i++)
            {
                float ra = half1.GetAt(i);
                float rb =
                    half2.X * AbsR[i, 0] +
                    half2.Y * AbsR[i, 1] +
                    half2.Z * AbsR[i, 2];

                float dist = MathF.Abs(t[i]);
                if (!TestAxis(A[i], ra + rb - dist))
                    return CollisionManifold.NoCollision();
            }

            // Axes B0..B2
            for (int j = 0; j < 3; j++)
            {
                float ra =
                    half1.X * AbsR[0, j] +
                    half1.Y * AbsR[1, j] +
                    half1.Z * AbsR[2, j];

                float rb = half2.GetAt(j);

                float dist = MathF.Abs(
                    t[0] * R[0, j] +
                    t[1] * R[1, j] +
                    t[2] * R[2, j]);

                if (!TestAxis(B[j], ra + rb - dist))
                    return CollisionManifold.NoCollision();
            }

            // Cross product axes
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Vector3 axis = Vector3.Cross(A[i], B[j]);
                    if (axis.LengthSquared() < 1e-8f)
                        continue;

                    float ra =
                        half1.GetAt((i + 1) % 3) * AbsR[(i + 2) % 3, j] +
                        half1.GetAt((i + 2) % 3) * AbsR[(i + 1) % 3, j];

                    float rb =
                        half2.GetAt((j + 1) % 3) * AbsR[i, (j + 2) % 3] +
                        half2.GetAt((j + 2) % 3) * AbsR[i, (j + 1) % 3];

                    float dist = MathF.Abs(
                        t[(i + 2) % 3] * R[(i + 1) % 3, j] -
                        t[(i + 1) % 3] * R[(i + 2) % 3, j]);

                    if (!TestAxis(Vector3.Normalize(axis), ra + rb - dist))
                        return CollisionManifold.NoCollision();
                }

            Vector3 normal = Vector3.Normalize(bestAxis);

            if (Vector3.Dot(normal, c2 - c1) < 0)
                normal = -normal;

            // Simple contact approximation
            Vector3 contactPoint = c1;// (c1 + c2) * 0.5f;

            return new CollisionManifold(new[] { contactPoint }, normal, penetration);
        }

        public static float GetAt(this Vector3 v, int i)
        {
            return i switch
            {
                0 => v.X,
                1 => v.Y,
                _ => v.Z
            };
        }
    }
}
