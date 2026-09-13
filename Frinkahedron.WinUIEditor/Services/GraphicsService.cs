using Frinkahedron.VeldridImplementation;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.WinUIEditor.Services
{
    internal sealed class GraphicsService
    {
        private static GraphicsService? service;
        private Task<IAssetManager> assetManagerTask;

        public GraphicsDevice GraphicsDevice { get; }

        private GraphicsService()
        {
            var options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = false,
                SyncToVerticalBlank = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
            };
            GraphicsDevice = GraphicsDevice.CreateD3D11(options);
            assetManagerTask = Task.Run<IAssetManager>(() => FromFolderAssetManager.LoadAssets(GraphicsDevice.ResourceFactory, GraphicsDevice, "C:\\Users\\Andy\\source\\repos\\Frinkahedron\\Frinkahedron.TestApp\\Assets")); // TODO Fix
        }

        public static GraphicsService Current => service ??= new GraphicsService(); // todo replace with service provider

        public async Task<IAssetManager> GetAssetManager() => await assetManagerTask;

        public Swapchain CreateSwapchain(SwapChainPanel panel)
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

            return GraphicsDevice.ResourceFactory.CreateSwapchain(swapChainDescription);
        }
    }
}
