using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Point
    {
        public TuioTime StartTime { get; protected set; }
        public Vector2 Position { get; protected set; }
        
        public Tuio20Point(TuioTime startTime, Vector2 position)
        {
            StartTime = startTime;
            Position = position;
        }
    }
}