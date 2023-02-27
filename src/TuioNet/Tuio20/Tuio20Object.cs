using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Object
    {
        public TuioTime StartTime { get; private set; }
        public TuioTime CurrentTime { get; private set; }
        public uint SessionId { get; private set; }
        public Tuio20Token Token { get; private set; }
        public Tuio20Pointer Pointer { get; private set; }
        public Tuio20Bounds Bounds { get; private set; }
        public Tuio20Symbol Symbol { get; private set; }
        public TuioState State { get; private set; }

        public Tuio20Object(TuioTime startTime, uint sessionId)
        {
            StartTime = startTime;
            CurrentTime = startTime;
            SessionId = sessionId;
            Token = null;
            Pointer = null;
            Bounds = null;
            Symbol = null;
            State = TuioState.Added;
        }
        
        public void SetTuioToken(Tuio20Token token)
        {
            Token = token;
            State = TuioState.Added;
        }

        public void SetTuioPointer(Tuio20Pointer pointer)
        {
            Pointer = pointer;
            State = TuioState.Added;
        }

        public void SetTuioBounds(Tuio20Bounds bounds)
        {
            Bounds = bounds;
            State = TuioState.Added;
        }

        public void SetTuioSymbol(Tuio20Symbol symbol)
        {
            Symbol = symbol;
            State = TuioState.Added;
        }
        
        public bool ContainsTuioToken()
        {
            return Token != null;
        }

        public bool ContainsTuioPointer()
        {
            return Pointer != null;
        }

        public bool ContainsTuioBounds()
        {
            return Bounds != null;
        }

        public bool ContainsTuioSymbol()
        {
            return Symbol != null;
        }
        
        public bool ContainsNewTuioToken()
        {
            return Token is {State: TuioState.Added};
        }

        public bool ContainsNewTuioPointer()
        {
            return Pointer is {State: TuioState.Added};
        }

        public bool ContainsNewTuioBounds()
        {
            return Bounds is {State: TuioState.Added};
        }

        public bool ContainsNewTuioSymbol()
        {
            return Symbol is {State: TuioState.Added};
        }
        
        internal void Update(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            State = TuioState.Idle;
        }

        internal void Remove(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            Token?._remove(currentTime);
            Pointer?._remove(currentTime);
            Bounds?._remove(currentTime);
            Symbol?._remove(currentTime);
            State = TuioState.Removed;
        }
    }
}