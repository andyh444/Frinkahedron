using Frinkahedron.Core.Template;
using Frinkahedron.WinUIEditor.ViewModels.RenderViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frinkahedron.WinUIEditor.Utilities;
using CommunityToolkit.Mvvm.Input;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    internal sealed class GameTemplateViewModel : ViewModelBase
    {
        private GameObjectTemplateViewModel activeGameObject;
        private RenderViewModelBase renderer;

        public WrappedObservableList<GameObjectTemplate, GameObjectTemplateViewModel> GameObjects { get; }

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

        public RelayCommand AddObjectCommand { get; }

        public GameTemplateViewModel(GameTemplate model)
        {
            Model = model;
            GameObjects = new WrappedObservableList<GameObjectTemplate, GameObjectTemplateViewModel>(model.GameObjects, model => new GameObjectTemplateViewModel(model));
            activeGameObject = GameObjects.First();

            renderer = new GameObjectEditorViewModel(ActiveGameObject);

            AddObjectCommand = new RelayCommand(() => GameObjects.Add(new GameObjectTemplate())); // todo this isn't updating in the UI - the CollectionChanged event isn't subscribed to
        }
    }
}
