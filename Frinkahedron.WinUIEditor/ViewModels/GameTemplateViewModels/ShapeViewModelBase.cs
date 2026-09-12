using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    internal abstract class ShapeViewModelBase : ViewModelBase, IEquatable<ShapeViewModelBase>
    {
        public abstract string DisplayName { get; }

        public abstract IShapeTemplate? Model { get; }

        public bool Equals(ShapeViewModelBase? other)
            => DisplayName == other?.DisplayName;

        public virtual IReadOnlyList<IGizmo> GetGizmos() => [];
    }

    public interface IGizmo
    {
        bool IsMouseOver(GameObject editableObject, GameState gameState);

        void OnDragged(GameObject editableObject, GameState gameState);

        void Draw(bool mouseOver, bool mouseDragged, GameObject editableObject, IRenderContext renderer);
    }
}
