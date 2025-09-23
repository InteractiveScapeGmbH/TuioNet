using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Component : Tuio20Point
    {
        private static int MAX_PATH_LENGTH = 128; 
        
        /// <summary>
        /// Time since creation as TuioTime.
        /// </summary>
        public TuioTime CurrentTime { get; protected set; }
        public Tuio20Object Container { get; protected set; }
        
        /// <summary>
        /// The rotation angle of the TuioComponent in radians.
        /// </summary>
        public float Angle { get; protected set; }
        
        /// <summary>
        /// The velocity vector of the TuioContainer.
        /// </summary>
        public Vector2 Velocity { get; protected set; }
        
        /// <summary>
        /// Calculated length of the velocity vector.
        /// </summary>
        public float Speed { get; protected set; }
        
        /// <summary>
        /// The speed of the rotation.
        /// </summary>
        public float RotationSpeed { get; protected set; }
        
        /// <summary>
        /// The current acceleration of the TuioComponent. Can be calculated or given by the TUIO-Sender.
        /// </summary>
        public float Acceleration { get; protected set; }
        
        /// <summary>
        /// The acceleration of the rotation. Amount of rotation change between to updates.
        /// </summary>
        public float RotationAcceleration { get; protected set; }
        
        /// <summary>
        /// The current state of the TuioComponent.
        /// </summary>
        public TuioState State { get; protected set; }

        protected readonly Queue<Tuio20Point> PrevPoints = new Queue<Tuio20Point>();

        public Tuio20Component(TuioTime startTime, Tuio20Object container, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration) : base(startTime, position)
        {
            CurrentTime = startTime;
            Container = container;
            Position = position;
            Angle = angle;
            Velocity = velocity;
            Speed = Velocity.Length();
            RotationSpeed = rotationSpeed;
            Acceleration = acceleration;
            RotationAcceleration = rotationAcceleration;
            State = TuioState.Added;
            PrevPoints.Enqueue(new Tuio20Point(startTime, Position));
        }
        
        public uint SessionId => Container.SessionId;
        
        public List<Tuio20Point> GetPath()
        {
            return PrevPoints.ToList();
        }

        internal void UpdateComponent(TuioTime currentTime, Vector2 position, float angle,
            Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            CurrentTime = currentTime;
            Position = position;
            Angle = angle;
            Velocity = velocity;
            Speed = Velocity.Length();
            RotationSpeed = rotationSpeed;
            Acceleration = acceleration;
            RotationAcceleration = rotationAcceleration;
            PrevPoints.Enqueue(new Tuio20Point(CurrentTime, Position));
            if (PrevPoints.Count > MAX_PATH_LENGTH)
            {
                PrevPoints.Dequeue();
            }
            if (Acceleration > 0)
            {
                State = TuioState.Accelerating;
            }
            else if (Acceleration < 0)
            {
                State = TuioState.Decelerating;
            }
            else if (RotationAcceleration != 0 && State == TuioState.Stopped)
            {
                State = TuioState.Rotating;
            }
            else 
            {
                State = TuioState.Stopped;
            }
            Container.Update(currentTime);
        }
        
        internal void Remove(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            State = TuioState.Removed;
        }
    }
}