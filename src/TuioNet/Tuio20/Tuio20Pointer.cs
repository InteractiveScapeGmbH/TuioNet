using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Pointer : Tuio20Component, ITuioEntity, ITouchDrawer
    {
        /// <summary>
        /// Allows distinction between different pointer input devices. The first two bytes encode the user id. The Last two bytes encode the type id.
        /// </summary>
        public int TypeUserId { get; private set; }
        
        /// <summary>
        /// Allows distinction of individual pointer components during a session.
        /// </summary>
        public int ComponentId { get; private set; }
        
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
        
        public Tuio20Pointer(TuioTime startTime, Tuio20Object container, int typeUserId, int componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration) : base(startTime, container, position, angle, velocity, 0, acceleration, 0)
        {
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Shear = shear;
            Radius = radius;
            Pressure = pressure;
            PressureSpeed = pressureSpeed;
            PressureAcceleration = pressureAcceleration;
        }
        
        internal bool HasChanged(int typeUserId, int componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration)
        {
            return !(typeUserId == TypeUserId && componentId == ComponentId && position == Position && angle == Angle && shear == Shear &&radius == Radius &&pressure == Pressure &&
                     velocity == Velocity && pressureSpeed == this.PressureSpeed && acceleration == base.Acceleration && pressureAcceleration == this.PressureAcceleration);
        }

        public void Update(TuioTime currentTime, int typeUserId, int componentId, Vector2 position, float angle, float shear, float radius, float pressure, Vector2 velocity, float pressureSpeed, float acceleration, float pressureAcceleration)
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
        
        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText =>
            $"Id: {SessionId}\nPosition: {Position:f2}";

        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio2/ptr");
                message.Append(SessionId);
                message.Append(TypeUserId);
                message.Append(ComponentId);
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