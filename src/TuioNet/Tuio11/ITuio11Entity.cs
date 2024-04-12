using OSC.NET;

namespace TuioNet.Tuio11
{
    public interface ITuio11Entity
    {
        public OSCMessage SetMessage { get; }
    }
}