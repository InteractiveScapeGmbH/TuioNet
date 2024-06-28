using OSC.NET;

namespace TuioNet.Common
{
    public interface ITuioEntity
    {
        public int SessionId { get; }
        public OSCMessage OscMessage { get; }

    }
}