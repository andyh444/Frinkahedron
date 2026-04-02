using Frinkahedron.Core;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class VeldridRenderContext : IRenderContext
    {
        public enum InstructionType
        {
            Model,
            Primitive
        }

        public readonly struct DrawInstruction
        {
            public required InstructionType InstructionType { get; init; }
            public string ModelID { get; init; }
            public Primitive Primitive { get; init; }
            public required Matrix4x4 Transform { get; init; }
        };

        private readonly List<DrawInstruction> drawInstructions;

        public IReadOnlyList<DrawInstruction> DrawInstructions => drawInstructions;

        public VeldridRenderContext()
        {
            drawInstructions = new List<DrawInstruction>();
        }

        public void DrawModel(string modelID, Matrix4x4 transform)
        {
            drawInstructions.Add(new DrawInstruction
            {
                InstructionType = InstructionType.Model,
                ModelID = modelID,
                Transform = transform
            });
        }

        public void DrawPrimitiveWireframe(Primitive primitive, Matrix4x4 transform)
        {
            drawInstructions.Add(new DrawInstruction
            {
                InstructionType = InstructionType.Primitive,
                Primitive = primitive,
                Transform = transform
            });
        }
    }
}
