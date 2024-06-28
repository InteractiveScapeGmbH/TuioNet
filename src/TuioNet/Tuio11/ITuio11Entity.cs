using OSC.NET;

namespace TuioNet.Tuio11
{
    public interface ITuio11Entity
    {
        public uint SessionId { get; }
        public OSCMessage SetMessage { get; }

    }
}