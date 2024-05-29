using Stride.Core.Mathematics;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Point
    {
        /// <summary>
        /// Creation time of the TuioPoint as TuioTime.
        /// </summary>
        public TuioTime StartTime { get; }
        
        /// <summary>
        /// The normalized position of the TuioPoint. [0..1]
        /// </summary>
        public Vector2 Position { get; protected set; }

        public Tuio11Point(TuioTime startTime, Vector2 position)
        {
            StartTime = startTime;
            Position = position;
        }
    }
}