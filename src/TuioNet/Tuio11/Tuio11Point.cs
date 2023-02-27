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
        public float xPos { get; protected set; }
        /// <summary>
        /// The normalized y-Position of the TuioPoint. [0..1]
        /// </summary>
        public float yPos { get; protected set; }

        public Tuio11Point(TuioTime startTime, float xPos, float yPos)
        {
            StartTime = startTime;
            this.xPos = xPos;
            this.yPos = yPos;
        }
    }
}