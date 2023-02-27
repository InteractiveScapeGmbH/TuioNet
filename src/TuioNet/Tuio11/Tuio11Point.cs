using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Point
    {
        public TuioTime StartTime { get; protected set; }
        public float xPos { get; protected set; }
        public float yPos { get; protected set; }
        
        public Tuio11Point(TuioTime startTime, float xPos, float yPos)
        {
            StartTime = startTime;
            this.xPos = xPos;
            this.yPos = yPos;
        }
    }
}