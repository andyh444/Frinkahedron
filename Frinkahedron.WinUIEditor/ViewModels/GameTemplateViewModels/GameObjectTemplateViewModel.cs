using Frinkahedron.Core.Template;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    internal sealed class GameObjectTemplateViewModel : ViewModelBase
    {
        private static int count;

        public event Action? ObjectChanged;

        public string Name { get; }

        public GameObjectTemplate Model { get; }

        public RenderableTemplateViewModel RenderableTemplate { get; }

        public ShapeViewModelBase Shape
        {
            get => GetShapeViewModel();
            set
            {
                if (value is null)
                {
                    // this gets hit when changing active game object template view model - todo figure out why
                    return;
                }
                SetModelProperty(GetShapeViewModel(), value, v => Model.Collider = v?.Model, nameof(Shape));
            }
        }

        public ObservableCollection<ShapeViewModelBase> AvailableShapes { get; }

        public GameObjectTemplateViewModel(GameObjectTemplate model)
        {
            Model = model;
            AvailableShapes = GetAvailableShapes(model.Collider);
            RenderableTemplate = new RenderableTemplateViewModel(Model.Renderable as ModelEntitiesRenderableTemplate ?? new ModelEntitiesRenderableTemplate());

            this.PropertyChanged += (o, e) => FireObjectChanged();
            foreach (var shape in AvailableShapes)
            {
                shape.PropertyChanged += (o, e) => FireObjectChanged();
            }
            RenderableTemplate.EnabledIndices.CollectionChanged += (o, e) => FireObjectChanged();
            RenderableTemplate.PropertyChanged += (o, e) => FireObjectChanged();
            RenderableTemplate.TransformTemplate.PropertyChanged += (o, e) => FireObjectChanged();

            Name = $"Object {count++}"; // todo replace with actual name
        }

        private void FireObjectChanged()
        {
            ObjectChanged?.Invoke();
        }

        private ObservableCollection<ShapeViewModelBase> GetAvailableShapes(IShapeTemplate? modelShape)
        {
            return new ObservableCollection<ShapeViewModelBase>
            {
                new UnsetShapeViewModel(),
                new BoxShapeViewModel(modelShape as BoxTemplate ?? new BoxTemplate()),
                new SphereShapeViewModel(modelShape as SphereTemplate ?? new SphereTemplate()),
            };
        }

        private ShapeViewModelBase GetShapeViewModel()
        {
            foreach (var shape in AvailableShapes)
            {
                if (shape.Model is null && Model.Collider is null)
                {
                    return shape;
                }
                if (shape.Model?.GetType() == Model.Collider?.GetType())
                {
                    return shape;
                }
            }
            throw new InvalidOperationException("Shouldn't get here");
        }
    }
}
