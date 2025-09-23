using TuioNet.OSC;

namespace TuioNet.Common
{
    public interface ITuioEntity
    {
        public uint SessionId { get; }
        public OSCMessage OscMessage { get; }

    }
}