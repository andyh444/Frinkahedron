using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
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
        private GraphicsDevice graphicsDevice;
        private Swapchain? swapchain;
        private GraphicsResources? graphicsResources;
        private Scene scene;
        private GameState gameState;
        private IAssetManager assetManager;
        private UserControlInputListener? inputListener;

        public MainWindow()
        {
            InitializeComponent();

            var options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = false,
                SyncToVerticalBlank = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
            };
            graphicsDevice = GraphicsDevice.CreateD3D11(options);

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            scene = CreateScene(1920f / 1080f);
            gameState = new GameState(0.01f, scene);
            assetManager = FromFolderAssetManager.LoadAssets(graphicsDevice.ResourceFactory, graphicsDevice, "C:\\Users\\Andy\\source\\repos\\Frinkahedron\\Frinkahedron.TestApp\\Assets"); // TODO Fix

            inputListener = new UserControlInputListener(grid);
        }

        private void CompositionTarget_Rendering(object? sender, object e)
        {
            if (inputListener is null)
            {
                return;
            }
            inputListener.UpdateInput(gameState.Input);
            scene.Update(gameState);
            Draw();
        }

        private void panel_Loaded(object sender, RoutedEventArgs e)
        {
            InitialiseSwapchain();
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            InitialiseSwapchain();
        }

        private void InitialiseSwapchain()
        {
            var swapChainSource = SwapchainSource.CreateWinUI3(panel, 96);
            var swapChainDescription = new SwapchainDescription
            {
                Source = swapChainSource,
                Width = (uint)panel.ActualSize.X,
                Height = (uint)panel.ActualSize.Y,
                SyncToVerticalBlank = true,
                DepthFormat = PixelFormat.D32_Float_S8_UInt
            };

            swapchain = graphicsDevice.ResourceFactory.CreateSwapchain(swapChainDescription);
            graphicsResources?.Dispose();
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)panel.ActualSize.X, (int)panel.ActualSize.Y, assetManager, swapchain);

            //bool focus = panel.Focus(FocusState.Programmatic);
        }

        private Scene CreateScene(float aspectRatio)
        {
            if (File.Exists($@"C:\tmp\tempgame.json"))
            {
                using var fs = File.OpenRead($@"C:\tmp\tempgame.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, };
                options.Converters.Add(new Vector3Converter());
                var template = JsonSerializer.Deserialize<GameTemplate>(fs, options);

                return template.Levels[0].ToScene(template, new Vector3(0, 0, -2), new Vector3(0, 0, 1), aspectRatio);
            }
            else
            {
                SceneBuilder sb = new SceneBuilder();
                sb.AddBigBoxes();
                sb.AddBasicCar();
                sb.AddCrateTower(new Vector3(0, -14, 0));

                var scene = sb.ToScene(new Vector3(0, 0, -2), new Vector3(0, 0, 1), aspectRatio);
                scene.SceneLights.PointLights.Add(new PointLight(new Vector3(), new Vector3(1), 100f));
                scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, -75), new Vector3(1, 0, 0), 200f));
                scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, 75), new Vector3(0, 1, 0), 300f));
                scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f)), new Vector3(1));

                return scene;
            }
        }

        private void Draw()
        {
            if (swapchain is null || graphicsResources is null)
            {
                return;
            }
            VeldridRenderContext context = new VeldridRenderContext();
            scene.Draw(context);

            graphicsResources.CommandList.Begin();
            foreach (var renderPass in graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsDevice, graphicsResources.CommandList, graphicsResources, scene, context.DrawInstructions);
            }
            graphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(graphicsResources.CommandList);
            graphicsDevice.SwapBuffers(swapchain);
        }
    }
}
