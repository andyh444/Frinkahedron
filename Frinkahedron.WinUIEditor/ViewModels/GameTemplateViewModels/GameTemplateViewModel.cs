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
        public ObservableCollection<GameObjectTemplateViewModel> GameObjects { get; }

        public GameObjectTemplateViewModel ActiveGameObject { get; }

        public RenderViewModelBase Renderer { get; }

        public GameTemplate Model { get; }

        public GameTemplateViewModel(GameTemplate model)
        {
            Model = model;
            GameObjects = new ObservableCollection<GameObjectTemplateViewModel>(model.GameObjects.Select(x => new GameObjectTemplateViewModel(x)));
            ActiveGameObject = GameObjects.First();

            Renderer = new GameObjectEditorViewModel(ActiveGameObject);
        }
    }
}
