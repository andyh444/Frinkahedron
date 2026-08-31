using CommunityToolkit.Mvvm.ComponentModel;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels
{
    internal class EnabledIndexViewModel(bool[] values, int index) : ViewModelBase
    {
        public bool IsChecked
        {
            get => values[index];
            set => SetModelProperty(values[index], value, v => values[index] = v);
        }

        public string Name => $"Entity {index}";
    }

    internal class RenderableTemplateViewModel : ViewModelBase
    {
        private readonly ObservableCollection<EnabledIndexViewModel> enabledIndices;
        private readonly ModelEntitiesRenderableTemplate model;

        public string ModelID
        {
            get => model.ModelID;
            set => SetModelProperty(model.ModelID, value, v => model.ModelID = v);
        }

        public ObservableCollection<EnabledIndexViewModel> EnabledIndices => enabledIndices;

        public TransformTemplateViewModel TransformTemplateViewModel { get; }

        public RenderableTemplateViewModel(ModelEntitiesRenderableTemplate model)
        {
            this.model = model;
            enabledIndices = new ObservableCollection<EnabledIndexViewModel>(this.model.EnabledIndices.Select((x, i) => new EnabledIndexViewModel(model.EnabledIndices, i)));
            TransformTemplateViewModel = new TransformTemplateViewModel(model.Transform);
        }
    }
}
