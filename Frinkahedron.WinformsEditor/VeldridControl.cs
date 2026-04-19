using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private long lastTimestamp;


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
            lastTimestamp = Stopwatch.GetTimestamp();
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
            TimeSpan timeSpan = Stopwatch.GetElapsedTime(lastTimestamp);
            Render(swapchain, timeSpan);
            lastTimestamp = Stopwatch.GetTimestamp();
        }

        protected abstract void Render(Swapchain swapChain, TimeSpan interval);
    }
}
