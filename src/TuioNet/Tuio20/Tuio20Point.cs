using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Point
    {
        /// <summary>
        /// Creation time of the TuioPoint as TuioTime.
        /// </summary>
        public TuioTime StartTime { get; }
        
        /// <summary>
        /// Time since creation as TuioTime.
        /// </summary>
        public TuioTime CurrentTime { get; protected set; }
        
        /// <summary>
        /// The normalized position of the TuioPoint. [0..1]
        /// </summary>
        public Vector2 Position { get; protected set; }
        
        public Tuio20Point(TuioTime startTime, Vector2 position)
        {
            StartTime = startTime;
            CurrentTime = startTime;
            Position = position;
        }
    }
}