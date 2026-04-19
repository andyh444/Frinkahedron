using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.WinformsEditor
{
    public abstract class VeldridControl : UserControl
    {
        private System.Windows.Forms.Timer timer;
        protected Swapchain? swapchain;

        public VeldridControl()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 10;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            swapchain?.Resize((uint)Width, (uint)Height);
        }

        protected void StartRenderLoop()
        {
            timer.Start();
        }

        public void Initialise(GraphicsService graphicsService)
        {
            swapchain = graphicsService.CreateSwapchain(this);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (swapchain is null)
            {
                return;
            }
            Render(swapchain, TimeSpan.FromMilliseconds(timer.Interval));
        }

        protected abstract void Render(Swapchain swapChain, TimeSpan interval);
    }
}
