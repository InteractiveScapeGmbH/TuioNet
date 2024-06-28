using System;
using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Object : Tuio11Container, ITuio11Entity, IObjectDrawer
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
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText => $"s_Id: {SessionId}\nId: {SymbolId}\nAngle: {(Angle * 180f / Math.PI):f2}\nPosition: {Position:f2}";


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
        
        internal bool HasChanged(Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            return !(position.X == ((Tuio11Point)this).Position.X && position.Y == ((Tuio11Point)this).Position.Y && angle == Angle && velocity.X == base.Velocity.X && velocity.Y == base.Velocity.Y &&
                     rotationSpeed == RotationSpeed && acceleration == Acceleration && rotationAcceleration == RotationAcceleration);
        }

        internal void Update(TuioTime currentTime, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (position.X != ((Tuio11Point)this).Position.X && velocity.X == 0) || (position.Y != ((Tuio11Point)this).Position.Y && velocity.Y == 0);
            UpdateContainer(currentTime, position, velocity, acceleration, isCalculateSpeeds);

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
                    RotationAcceleration = (RotationSpeed - lastRotationSpeed) / dt;
                }
            }
            else
            {
                RotationSpeed = rotationSpeed;
                RotationAcceleration = rotationAcceleration;
            }
            Angle = angle;

            if (State != TuioState.Stopped && RotationAcceleration != 0)
            {
                State = TuioState.Rotating;
            }
        }

        public OSCMessage SetMessage
        {
            get
            {
                var message = new OSCMessage("/tuio/2Dobj");
                message.Append("set");
                message.Append(SessionId);
                message.Append(SymbolId);
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