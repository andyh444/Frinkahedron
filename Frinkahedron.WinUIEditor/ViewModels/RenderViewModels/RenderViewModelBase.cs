using CommunityToolkit.Mvvm.ComponentModel;
using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.WinUIEditor.ViewModels.RenderViewModels
{
    internal abstract class RenderViewModelBase : ViewModelBase
    {
        public abstract void Initialise(GraphicsDevice graphicsDevice, Vector2 initialSize, Swapchain swapchain);

        public abstract void SizeChanged(GraphicsDevice graphicsDevice, Vector2 newSize, Swapchain swapchain);

        public abstract void Update(Action<Input> updateInput);

        public abstract void Draw(GraphicsDevice graphicsDevice, Swapchain swapchain);
    }
}
