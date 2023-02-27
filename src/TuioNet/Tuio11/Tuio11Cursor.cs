using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Cursor : Tuio11Container
    {
        /// <summary>
        /// Individual cursor ID assigned to each TuioCursor.
        /// </summary>
        public uint CursorId { get; protected set; }
        
        public Tuio11Cursor(TuioTime startTime, uint sessionId, uint cursorId, float posX, float posY, float speedX, float speedY, float motionAccel) : base(startTime, sessionId, posX, posY, speedX, speedY, motionAccel)
        {
            CursorId = cursorId;
        }
        
        internal bool HasChanged(float posX, float posY, float speedX, float speedY, float motionAccel)
        {
            return !(posX == ((Tuio11Point)this).PosX && posY == ((Tuio11Point)this).PosY && speedX == base.SpeedX && speedY == base.SpeedY && motionAccel == MotionAccel);
        }

        internal void Update(TuioTime currentTime, float posX, float posY, float speedX, float speedY, float motionAccel)
        {
            var isCalculateSpeeds = (posX != ((Tuio11Point)this).PosX && speedX == 0) || (posY != ((Tuio11Point)this).PosY && speedY == 0);
            UpdateContainer(currentTime, posX, posY, speedX, speedY, motionAccel, isCalculateSpeeds);
        }
    }
}