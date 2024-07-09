using System;
using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Token : Tuio20Component, ITuioEntity, IObjectDrawer
    {
        /// <summary>
        /// Allows multiplexing of various symbol types and association of additional user id. First two bytes encode user id. Last two bytes encode type id.
        /// User id 0 is reserved for "no user".
        /// </summary>
        public uint TypeUserId { get; private set; }
        
        /// <summary>
        /// Allows distinction of individual tokens during a session.
        /// </summary>
        public uint ComponentId { get; private set; }
        
        public Tuio20Token(TuioTime startTime, Tuio20Object container, uint typeUserId, uint componentId, Vector2 position, float angle, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration) : base(startTime, container, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration)
        {
            TypeUserId = typeUserId;
            ComponentId = componentId;
        }
        
        internal bool HasChanged(uint typeUserId, uint componentId, Vector2 position, float angle, Vector2 velocity,
            float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            return !(typeUserId == TypeUserId && componentId == ComponentId && position == Position && angle == Angle &&
                     velocity == Velocity && rotationSpeed == RotationSpeed && acceleration == Acceleration && rotationAcceleration == RotationAcceleration);
        }

        public void Update(TuioTime currentTime, uint typeUserId, uint componentId, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            UpdateComponent(currentTime, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration);
            TypeUserId = typeUserId;
            ComponentId = componentId;
        }

        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText => $"s_Id: {SessionId}\nId: {ComponentId}\nAngle: {(Angle * 180f / Math.PI):f2}\nPosition: {Position:f2}";

        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio2/tok");
                message.Append(SessionId);
                message.Append(TypeUserId);
                message.Append(ComponentId);
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