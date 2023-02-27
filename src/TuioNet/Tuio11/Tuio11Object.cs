using System;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Object : Tuio11Container
    {
        public uint SymbolId { get; protected set; }
        public float Angle { get; protected set; }
        public float RotationSpeed { get; protected set; }
        public float RotationAccel { get; protected set; }
        
        public Tuio11Object(TuioTime startTime, uint sessionId, uint symbolId, float xPos, float yPos, float angle, float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel) : base(startTime, sessionId, xPos, yPos, xSpeed, ySpeed, motionAccel)
        {
            SymbolId = symbolId;
            Angle = angle;
            RotationSpeed = rotationSpeed;
            RotationAccel = rotationAccel;
        }
        
        internal bool HasChanged(float xPos, float yPos, float angle, float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            return !(xPos == ((Tuio11Point)this).xPos && yPos == ((Tuio11Point)this).yPos && angle == Angle && xSpeed == base.xSpeed && ySpeed == base.ySpeed &&
                     rotationSpeed == RotationSpeed && motionAccel == MotionAccel && rotationAccel == RotationAccel);
        }

        internal void Update(TuioTime currentTime, float xPos, float yPos, float angle,
            float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (xPos != ((Tuio11Point)this).xPos && xSpeed == 0) || (yPos != ((Tuio11Point)this).yPos && ySpeed == 0);
            UpdateContainer(currentTime, xPos, yPos, xSpeed, ySpeed, motionAccel, isCalculateSpeeds);

            var isCalculateRotation = angle != Angle && rotationSpeed == 0;
            if(isCalculateRotation)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                if (dt > 0)
                {
                    var lastAngle = Angle;
                    var lastRotationSpeed = RotationSpeed;
                    var da = (angle - lastAngle) / (2 * (float)Math.PI);
                    if (da > 0.5f)
                    {
                        da -= 1;
                    } 
                    else if (da <= -0.5f)
                    {
                        da += 1;
                    }
                    RotationSpeed = da / dt;
                    RotationAccel = (RotationSpeed - lastRotationSpeed) / dt;
                }
            }
            else
            {
                RotationSpeed = rotationSpeed;
                RotationAccel = rotationAccel;
            }
            Angle = angle;

            if (State != TuioState.Stopped && RotationAccel != 0)
            {
                State = TuioState.Rotating;
            }
        }
    }
}