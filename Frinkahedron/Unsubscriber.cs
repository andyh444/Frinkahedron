using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public sealed class Unsubscriber(Action unsubscribeAction) : IDisposable
    {
        public void Dispose()
        {
            unsubscribeAction();
        }
    }
}
