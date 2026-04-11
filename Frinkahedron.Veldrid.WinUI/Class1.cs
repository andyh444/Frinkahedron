using Vortice.DXGI;

namespace Frinkahedron.Veldrid.WinUI
{
    public static class WinUISwapChain
    {
        public static CreateD3D11GraphicsDevice(object swapChainPanel, float logicalDpi)
        {
            float pixelScale = logicalDpi / 96.0f;

            // Properties of the swap chain
            SwapChainDescription1 swapChainDescription = new SwapChainDescription1()
            {
                AlphaMode = AlphaMode.Ignore,
                BufferCount = 2,
                Format = _colorFormat,
                Height = (int)(description.Height * pixelScale),
                Width = (int)(description.Width * pixelScale),
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.FlipSequential,
                Usage = Vortice.DXGI.Usage.RenderTargetOutput,
            };

            // Get the Vortice.DXGI factory automatically created when initializing the Direct3D device.
            using (IDXGIFactory2 dxgiFactory = _gd.Adapter.GetParent<IDXGIFactory2>())
            {
                // Create the swap chain and get the highest version available.
                using (IDXGISwapChain1 swapChain1 = dxgiFactory.CreateSwapChainForComposition(_gd.Device, swapChainDescription))
                {
                    _dxgiSwapChain = swapChain1.QueryInterface<IDXGISwapChain2>();
                }
            }

            ComObject co = new ComObject(winuiSource.SwapChainPanelNative);

            Vortice.WinUI.ISwapChainPanelNative swapchainPanelNative = co.QueryInterfaceOrNull<Vortice.WinUI.ISwapChainPanelNative>();
            if (swapchainPanelNative != null)
            {
                swapchainPanelNative.SetSwapChain(_dxgiSwapChain);
            }
            else
            {
                ISwapChainBackgroundPanelNative bgPanelNative = co.QueryInterfaceOrNull<ISwapChainBackgroundPanelNative>();
                if (bgPanelNative != null)
                {
                    bgPanelNative.SetSwapChain(_dxgiSwapChain);
                }
            }
        }
    }
}
