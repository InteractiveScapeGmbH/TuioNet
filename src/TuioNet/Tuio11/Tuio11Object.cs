using System;
using System.Numerics;
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
        
        public Tuio11Object(TuioTime startTime, uint sessionId, uint symbolId, Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float motionAccel, float rotationAccel) : base(startTime, sessionId, position, velocity, motionAccel)
        {
            SymbolId = symbolId;
            Angle = angle;
            RotationSpeed = rotationSpeed;
            RotationAccel = rotationAccel;
        }
        
        internal bool HasChanged(Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            return !(position.X == ((Tuio11Point)this).Position.X && position.Y == ((Tuio11Point)this).Position.Y && angle == Angle && velocity.X == base.Velocity.X && velocity.Y == base.Velocity.Y &&
                     rotationSpeed == RotationSpeed && motionAccel == MotionAccel && rotationAccel == RotationAccel);
        }

        internal void Update(TuioTime currentTime, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (position.X != ((Tuio11Point)this).Position.X && velocity.X == 0) || (position.Y != ((Tuio11Point)this).Position.Y && velocity.Y == 0);
            UpdateContainer(currentTime, position, velocity, motionAccel, isCalculateSpeeds);

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