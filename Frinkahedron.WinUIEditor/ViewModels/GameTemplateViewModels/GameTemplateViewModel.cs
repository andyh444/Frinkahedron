using Frinkahedron.Core.Template;
using Frinkahedron.WinUIEditor.ViewModels.RenderViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    internal sealed class GameTemplateViewModel : ViewModelBase
    {
        private GameObjectTemplateViewModel activeGameObject;
        private RenderViewModelBase renderer;

        public ObservableCollection<GameObjectTemplateViewModel> GameObjects { get; }

        public GameObjectTemplateViewModel ActiveGameObject
        {
            get => activeGameObject;
            set
            {
                activeGameObject = value;
                OnPropertyChanged(nameof(ActiveGameObject));

                Renderer = new GameObjectEditorViewModel(ActiveGameObject);
            }
        }

        public RenderViewModelBase Renderer
        {
            get => renderer;
            set
            {
                renderer = value;
                OnPropertyChanged(nameof(Renderer));
            }
        }

        public GameTemplate Model { get; }

        public GameTemplateViewModel(GameTemplate model)
        {
            Model = model;
            GameObjects = new ObservableCollection<GameObjectTemplateViewModel>(model.GameObjects.Select(x => new GameObjectTemplateViewModel(x)));
            activeGameObject = GameObjects.First();

            renderer = new GameObjectEditorViewModel(ActiveGameObject);
        }
    }
}
