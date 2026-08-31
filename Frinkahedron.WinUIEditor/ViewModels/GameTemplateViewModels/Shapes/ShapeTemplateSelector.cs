using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes
{
    internal sealed class ShapeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BoxTemplate { get; set; }
        public DataTemplate? SphereTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplate(item);
        }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            return item switch
            {
                BoxShapeViewModel => BoxTemplate,
                SphereShapeViewModel => SphereTemplate,
                _ => null
            };
        }
    }
}
