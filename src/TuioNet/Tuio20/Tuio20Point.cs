using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Point
    {
        public TuioTime StartTime { get; protected set; }
        public float xPos { get; protected set; }
        public float yPos { get; protected set; }
        
        public Tuio20Point(TuioTime startTime, float xPos, float yPos)
        {
            StartTime = startTime;
            this.xPos = xPos;
            this.yPos = yPos;
        }
        
    }
}