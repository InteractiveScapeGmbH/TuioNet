using System.Numerics;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Tuio20
{
    public class Tuio20Pointer : Tuio20Component, ITuioEntity
    {
        /// <summary>
        /// Allows distinction between different pointer input devices. The first two bytes encode the user id. The Last two bytes encode the type id.
        /// </summary>
        public uint TypeUserId { get; private set; }
        
        /// <summary>
        /// Allows distinction of individual pointer components during a session.
        /// </summary>
        public uint ComponentId { get; private set; }
        
        /// <summary>
        /// The shear angle relative to the horizontal surface plane.
        /// </summary>
        public float Shear { get; private set; }
        
        /// <summary>
        /// The radius indicates a pointers "region of influence".
        /// </summary>
        public float Radius { get; private set; }
        
        /// <summary>
        /// A normalized surface pressure value. If value is negative the pointer is hovering over the surface.
        /// </summary>
        public float Pressure { get; private set; }
        
        /// <summary>
        /// The speed with which the pressure is applied.
        /// </summary>
        public float PressureSpeed { get; private set; }
        
        /// <summary>
        /// The acceleration with which the pressure is applied.
        /// </summary>
        public float PressureAcceleration { get; private set; }
        
        public Tuio20Pointer(TuioTime startTime, Tuio20Object container, uint typeUserId, uint componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration) : base(startTime, container, position, angle, velocity, 0, acceleration, 0)
        {
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Shear = shear;
            Radius = radius;
            Pressure = pressure;
            PressureSpeed = pressureSpeed;
            PressureAcceleration = pressureAcceleration;
        }
        
        public bool HasChanged(uint typeUserId, uint componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration)
        {
            return !(typeUserId == TypeUserId && componentId == ComponentId && position == Position && angle == Angle && shear == Shear &&radius == Radius &&pressure == Pressure &&
                     velocity == Velocity && pressureSpeed == this.PressureSpeed && acceleration == base.Acceleration && pressureAcceleration == this.PressureAcceleration);
        }

        public void Update(TuioTime currentTime, uint typeUserId, uint componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration)
        {
            UpdateComponent(currentTime, position, angle, velocity, 0, acceleration, 0);
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Shear = shear;
            Radius = radius;
            Pressure = pressure;
            PressureSpeed = pressureSpeed;
            PressureAcceleration = pressureAcceleration;
        }
        
        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio2/ptr");
                message.Append((int)SessionId);
                message.Append((int)TypeUserId);
                message.Append((int)ComponentId);
                message.Append(Position.X);
                message.Append(Position.Y);
                message.Append(Angle);
                message.Append(Shear);
                message.Append(Radius);
                message.Append(Pressure);
                message.Append(Velocity.X);
                message.Append(Velocity.Y);
                message.Append(PressureSpeed);
                message.Append(Acceleration);
                message.Append(PressureAcceleration);
                return message;
            }
        }
    }
}