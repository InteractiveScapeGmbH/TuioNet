using Stride.Core.Mathematics;
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
        /// The normalized position of the TuioPoint. [0..1]
        /// </summary>
        public Vector2 Position { get; protected set; }
        
        public Tuio20Point(TuioTime startTime, Vector2 position)
        {
            StartTime = startTime;
            Position = position;
        }
    }
}