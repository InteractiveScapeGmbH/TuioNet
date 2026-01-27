using System.Collections.Generic;
using TuioNet.OSC;

namespace TuioNet.Server
{
    public interface ITuioManager
    {
        public IList<OSCBundle> FrameBundles { get; }
        public void Update();
        public void Quit();
    }
}

