using System;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Object : Tuio11Container
    {
        /// <summary>
        /// The individual symbol ID which is assigned to each TuioObject.
        /// </summary>
        public uint SymbolId { get; protected set; }
        
        /// <summary>
        /// The rotation angle of the TuioObject in radians.
        /// </summary>
        public float Angle { get; protected set; }
        
        /// <summary>
        /// The speed of the rotation.
        /// </summary>
        public float RotationSpeed { get; protected set; }
        
        /// <summary>
        /// The acceleration of the rotation. Amount of rotation change between to updates.
        /// </summary>
        public float RotationAccel { get; protected set; }
        
        public Tuio11Object(TuioTime startTime, uint sessionId, uint symbolId, float posX, float posY, float angle, float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel) : base(startTime, sessionId, posX, posY, speedX, speedY, motionAccel)
        {
            SymbolId = symbolId;
            Angle = angle;
            RotationSpeed = rotationSpeed;
            RotationAccel = rotationAccel;
        }
        
        internal bool HasChanged(float posX, float posY, float angle, float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            return !(posX == ((Tuio11Point)this).PosX && posY == ((Tuio11Point)this).PosY && angle == Angle && speedX == base.SpeedX && speedY == base.SpeedY &&
                     rotationSpeed == RotationSpeed && motionAccel == MotionAccel && rotationAccel == RotationAccel);
        }

        internal void Update(TuioTime currentTime, float posX, float posY, float angle,
            float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (posX != ((Tuio11Point)this).PosX && speedX == 0) || (posY != ((Tuio11Point)this).PosY && speedY == 0);
            UpdateContainer(currentTime, posX, posY, speedX, speedY, motionAccel, isCalculateSpeeds);

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