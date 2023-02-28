using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Pointer : Tuio20Component
    {
        public uint TypeId { get; private set; }
        public uint ComponentId { get; private set; }
        public float Shear { get; private set; }
        public float Radius { get; private set; }
        public float Press { get; private set; }
        public float PressureVelocity { get; private set; }
        public float PressureAcceleration { get; private set; }
        
        public Tuio20Pointer(TuioTime startTime, Tuio20Object container, uint typeId, uint componentId, Vector2 position, float angle, float shear, float radius, float press, Vector2 velocity, float pressureVelocity, float acceleration, float pressureAcceleration) : base(startTime, container, position, angle, velocity, 0, acceleration, 0)
        {
            TypeId = typeId;
            ComponentId = componentId;
            Shear = shear;
            Radius = radius;
            Press = press;
            PressureVelocity = pressureVelocity;
            PressureAcceleration = pressureAcceleration;
        }
        
        internal bool HasChanged(uint typeId, uint componentId, Vector2 position, float angle, float shear, float radius, float press, Vector2 velocity, float pressureVelocity, float acceleration, float pressureAcceleration)
        {
            return !(typeId == TypeId && componentId == ComponentId && position == Position && angle == Angle && shear == Shear &&radius == Radius &&press == Press &&
                     velocity == Velocity && pressureVelocity == this.PressureVelocity && acceleration == base.Acceleration && pressureAcceleration == this.PressureAcceleration);
        }

        internal void Update(TuioTime currentTime, uint typeId, uint componentId, Vector2 position, float angle, float shear, float radius, float press, Vector2 velocity, float pressureVelocity, float acceleration, float pressureAcceleration)
        {
            UpdateComponent(currentTime, position, angle, velocity, 0, acceleration, 0);
            TypeId = typeId;
            ComponentId = componentId;
            Shear = shear;
            Radius = radius;
            Press = press;
            PressureVelocity = pressureVelocity;
            PressureAcceleration = pressureAcceleration;
        }
    }
}