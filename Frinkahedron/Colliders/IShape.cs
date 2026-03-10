using Frinkahedron.Core.Maths;
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
        public void Draw(IRenderer renderer, Matrix4x4 position);

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass);

        public float CalculateVolume();
    }
}
