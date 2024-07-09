using OSC.NET;

namespace TuioNet.Common
{
    public interface ITuioEntity
    {
        public uint SessionId { get; }
        public OSCMessage OscMessage { get; }

    }
}