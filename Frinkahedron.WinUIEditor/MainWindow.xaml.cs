using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinUIEditor.ViewModels;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Veldrid;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Frinkahedron.WinUIEditor
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel viewModel = new MainViewModel();

            templateView.DataContext = viewModel.GameObjectTemplate;
            veldridPanel.SetViewModel(viewModel.Renderer);
        }

        
    }
}
