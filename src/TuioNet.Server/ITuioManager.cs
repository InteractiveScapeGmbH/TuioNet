using TuioNet.OSC;

namespace TuioNet.Server
{
    public interface ITuioManager
    {
        public OSCBundle[] FrameBundles { get; }
        public void Update();
        public void Quit();
    }
}

