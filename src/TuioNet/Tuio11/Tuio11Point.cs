using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Point
    {
        /// <summary>
        /// Creation time of the TuioPoint as TuioTime.
        /// </summary>
        public TuioTime StartTime { get; protected set; }
        
        /// <summary>
        /// The normalized x-Positon of the TuioPoint. [0..1]
        /// </summary>
        public float PosX { get; protected set; }
        
        /// <summary>
        /// The normalized y-Position of the TuioPoint. [0..1]
        /// </summary>
        public float PosY { get; protected set; }
        
        public Vector2 Position { get; protected set; }

        public Tuio11Point(TuioTime startTime, float posX, float posY)
        {
            StartTime = startTime;
            PosX = posX;
            PosY = posY;
            Position = new Vector2(posX, posY);
        }
    }
}