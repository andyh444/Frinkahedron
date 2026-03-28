using Frinkahedron;
using Frinkahedron.Core;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

internal class Program
{
    private static void Main(string[] args)
    {
        TestApp app = new TestApp();
        app.Run();
    }
}
