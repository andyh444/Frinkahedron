using Frinkahedron.Core.Template;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels;
using Frinkahedron.WinUIEditor.ViewModels.RenderViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public GameObjectTemplateViewModel GameObjectTemplate { get; }

        public RenderViewModelBase Renderer { get; }

        public MainViewModel()
        {
            GameObjectTemplate = new GameObjectTemplateViewModel(GetTemplate());
            Renderer = new GameObjectEditorViewModel(GameObjectTemplate);
        }

        private GameObjectTemplate GetTemplate()
        {
            if (File.Exists($@"C:\tmp\tempgame.json"))
            {
                using var fs = File.OpenRead($@"C:\tmp\tempgame.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, };
                options.Converters.Add(new Vector3Converter());
                var template = JsonSerializer.Deserialize<GameTemplate>(fs, options);

                return template?.GameObjects.FirstOrDefault() ?? new GameObjectTemplate();
            }
            else
            {
                return new GameObjectTemplate();
            }
        }
    }
}
