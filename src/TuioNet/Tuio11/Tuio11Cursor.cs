using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Cursor : Tuio11Container
    {
        public uint CursorId { get; protected set; }
        
        public Tuio11Cursor(TuioTime startTime, uint sessionId, uint cursorId, float xPos, float yPos, float xSpeed, float ySpeed, float motionAccel) : base(startTime, sessionId, xPos, yPos, xSpeed, ySpeed, motionAccel)
        {
            CursorId = cursorId;
        }
        
        public bool HasChanged(float xPos, float yPos, float xSpeed, float ySpeed, float motionAccel)
        {
            return !(xPos == ((Tuio11Point)this).xPos && yPos == ((Tuio11Point)this).yPos && xSpeed == base.xSpeed && ySpeed == base.ySpeed && motionAccel == MotionAccel);
        }

        public void Update(TuioTime currentTime, float xPos, float yPos, float xSpeed, float ySpeed, float motionAccel)
        {
            var isCalculateSpeeds = (xPos != ((Tuio11Point)this).xPos && xSpeed == 0) || (yPos != ((Tuio11Point)this).yPos && ySpeed == 0);
            UpdateContainer(currentTime, xPos, yPos, xSpeed, ySpeed, motionAccel, isCalculateSpeeds);
        }
    }
}