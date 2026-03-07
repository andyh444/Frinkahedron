using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    public static class RandomExtensions
    {
        public static float NextSingle(this Random random, float low, float high)
        {
            float range = high - low;
            return low + range * random.NextSingle();
        }

        public static Vector3 NextUnitVector(this Random random)
        {
            Vector3 vec = new Vector3(
                random.NextSingle(-1f, 1f),
                random.NextSingle(-1f, 1f),
                random.NextSingle(-1f, 1f));

            return Vector3.Normalize(vec);
        }
    }
}
