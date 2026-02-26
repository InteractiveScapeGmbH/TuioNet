using System;
using System.Numerics;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Tuio20
{
    public class Tuio20Bounds: Tuio20Component, ITuioEntity
    {
        /// <summary>
        /// Dimensions of the major and minor axis.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// The region area of the bounds.
        /// </summary>
        public float Area { get; private set; }
        
        public Tuio20Bounds(TuioTime startTime, Tuio20Object container, Vector2 position, float angle, Vector2 size, float area, Vector2? velocity = null, float rotationSpeed = 0f, float acceleration = 0f, float rotationAcceleration = 0f) : base(startTime, container, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration)
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