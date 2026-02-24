using System;
using System.Numerics;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Tuio11
{
    public class Tuio11Object : Tuio11Container, ITuioEntity
    {
        /// <summary>
        /// The individual symbol ID which is assigned to each TuioObject.
        /// </summary>
        public uint SymbolId { get; }
        
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
        public float RotationAcceleration { get; protected set; }
        
        public Tuio11Object(TuioTime startTime, uint sessionId, uint symbolId, Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration) : base(startTime, sessionId, position, velocity, acceleration)
        {
            SymbolId = symbolId;
            Angle = angle;
            RotationSpeed = rotationSpeed;
            RotationAcceleration = rotationAcceleration;
        }
        
        public bool HasChanged(Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            return !(position.X == ((Tuio11Point)this).Position.X && position.Y == ((Tuio11Point)this).Position.Y && angle == Angle && velocity.X == base.Velocity.X && velocity.Y == base.Velocity.Y &&
                     rotationSpeed == RotationSpeed && acceleration == Acceleration && rotationAcceleration == RotationAcceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position, float angle)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            UpdateContainer(currentTime, position);
            var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
            Angle = angle;
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
                RotationAcceleration = (RotationSpeed - lastRotationSpeed) / dt;
            }
            
            if (State != TuioState.Stopped && RotationAcceleration != 0)
            {
                State = TuioState.Rotating;
            }
        }

        public void Update(TuioTime currentTime, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            UpdateContainer(currentTime, position, velocity, acceleration);

            RotationSpeed = rotationSpeed;
            RotationAcceleration = rotationAcceleration;
            Angle = angle;

            if (State != TuioState.Stopped && RotationAcceleration != 0)
            {
                State = TuioState.Rotating;
            }
        }

        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio/2Dobj");
                message.Append("set");
                message.Append((int)SessionId);
                message.Append((int)SymbolId);
                message.Append(Position.X);
                message.Append(Position.Y);
                message.Append(Angle);
                message.Append(Velocity.X);
                message.Append(Velocity.Y);
                message.Append(RotationSpeed);
                message.Append(Acceleration);
                message.Append(RotationAcceleration);
                return message;
            }
        }
    }
}