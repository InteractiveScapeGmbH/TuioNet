using System;
using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Bounds: Tuio20Component, ITuioEntity, IBoundsDrawer
    {
        /// <summary>
        /// Dimensions of the major and minor axis.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText => $"s_Id: {SessionId}\nAngle: {(Angle * 180f / Math.PI):f2}\nPosition: {Position:f2}Size: {Size:f2}";

        /// <summary>
        /// The region area of the bounds.
        /// </summary>
        public float Area { get; private set; }
        
        public Tuio20Bounds(TuioTime startTime, Tuio20Object container, Vector2 position, float angle, Vector2 size, float area, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration) : base(startTime, container, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration)
        {
            Size = size;
            Area = area;
        }
        
        public bool HasChanged(Vector2 position, float angle, Vector2 size, float area, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            return !(position == Position && angle == Angle && size == Size && area == Area &&
                     velocity == Velocity && rotationSpeed == RotationSpeed && acceleration == Acceleration && rotationAcceleration == RotationAcceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position, float angle, Vector2 size, float area, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            UpdateComponent(currentTime, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration);
            Size = size;
            Area = area;
        }

        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio2/bnd");
                message.Append((int)SessionId);
                message.Append(Position.X);
                message.Append(Position.Y);
                message.Append(Angle);
                message.Append(Size.X);
                message.Append(Size.Y);
                message.Append(Area);
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