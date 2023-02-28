using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Bounds: Tuio20Component
    {
        public Vector2 Size { get; private set; }
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
    }
}