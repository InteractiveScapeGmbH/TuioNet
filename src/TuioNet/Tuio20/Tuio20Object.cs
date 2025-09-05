using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Object
    {
        /// <summary>
        /// Creation time of the TuioObject as TuioTime.
        /// </summary>
        public TuioTime StartTime { get; private set; }
        
        /// <summary>
        /// Time since creation as TuioTime.
        /// </summary>
        public TuioTime CurrentTime { get; private set; }
        
        /// <summary>
        /// The Session ID, a temporary unique object ID.
        /// </summary>
        public uint SessionId { get; private set; }
        
        /// <summary>
        /// The token associated with this object.
        /// </summary>
        public Tuio20Token Token { get; private set; }
        
        /// <summary>
        /// The pointer associated with this object.
        /// </summary>
        public Tuio20Pointer Pointer { get; private set; }
        
        /// <summary>
        /// The bounds associated with this object.
        /// </summary>
        public Tuio20Bounds Bounds { get; private set; }
        
        /// <summary>
        /// The symbol associated with this object.
        /// </summary>
        public Tuio20Symbol Symbol { get; private set; }
        
        /// <summary>
        /// The current state of the TuioContainer.
        /// </summary>
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
        
        public void Update(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            State = TuioState.Idle;
        }

        public void Remove(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            Token?.Remove(currentTime);
            Pointer?.Remove(currentTime);
            Bounds?.Remove(currentTime);
            Symbol?.Remove(currentTime);
            State = TuioState.Removed;
        }
    }
}