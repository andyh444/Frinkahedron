using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinUIEditor.Services;
using Frinkahedron.WinUIEditor.ViewModels;
using Frinkahedron.WinUIEditor.ViewModels.RenderViewModels;
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
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;

namespace Frinkahedron.WinUIEditor.Views;

public sealed partial class VeldridView : UserControl
{
    private Swapchain? swapchain;
    private UserControlInputListener inputListener;

    public VeldridView()
    {
        InitializeComponent();
        var graphicsDevice = GraphicsService.Current.GraphicsDevice;
        CompositionTarget.Rendering += CompositionTarget_Rendering;
        inputListener = new UserControlInputListener(this);
    }

    private RenderViewModelBase? GetRenderViewModel() => DataContext as RenderViewModelBase;

    private void CompositionTarget_Rendering(object? sender, object e)
    {
        var renderViewModel = GetRenderViewModel();
        if (renderViewModel is null)
        {
            return;
        }
        renderViewModel.Update(inputListener.UpdateInput);
        Draw();
    }

    private void renderPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var renderViewModel = GetRenderViewModel();
        if (renderViewModel is null || swapchain is null)
        {
            return;
        }
        var gd = GraphicsService.Current.GraphicsDevice;
        renderViewModel.SizeChanged(gd, renderPanel.ActualSize, swapchain);
        swapchain.Resize((uint)renderPanel.ActualSize.X, (uint)renderPanel.ActualSize.Y);
    }

    private async void VeldridView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        var renderViewModel = GetRenderViewModel();
        var gd = GraphicsService.Current.GraphicsDevice;
        swapchain = GraphicsService.Current.CreateSwapchain(renderPanel);
        await renderViewModel.Initialise(gd, ActualSize, swapchain);
    }

    private async void renderPanel_Loaded(object sender, RoutedEventArgs e)
    {
        var renderViewModel = GetRenderViewModel();
        var gd = GraphicsService.Current.GraphicsDevice;
        swapchain = GraphicsService.Current.CreateSwapchain(renderPanel);
        await renderViewModel.Initialise(gd, ActualSize, swapchain);

        DataContextChanged += VeldridView_DataContextChanged;
    }

    private void Draw()
    {
        var renderViewModel = GetRenderViewModel();
        if (swapchain is null || renderViewModel is null)
        {
            return;
        }
        var gd = GraphicsService.Current.GraphicsDevice;
        renderViewModel.Draw(gd, swapchain);
    }
}
