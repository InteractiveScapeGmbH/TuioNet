﻿using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Token : Tuio20Component
    {
        public uint TypeUserId { get; private set; }
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

        internal void Update(TuioTime currentTime, uint typeUserId, uint componentId, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            UpdateComponent(currentTime, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration);
            TypeUserId = typeUserId;
            ComponentId = componentId;
        }
    }
}