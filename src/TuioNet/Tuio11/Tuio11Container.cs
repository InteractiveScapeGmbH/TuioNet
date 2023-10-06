using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Container : Tuio11Point
    {
        private const int MAX_PATH_LENGTH = 128;

        /// <summary>
        /// The Session ID, a temporary unique object ID.
        /// </summary>
        public uint SessionId { get; }
        
        /// <summary>
        /// The velocity vector of the TuioContainer.
        /// </summary>
        public Vector2 Velocity { get; protected set; }
        
        /// <summary>
        /// Calculated length of the velocity vector.
        /// </summary>
        public float Speed { get; protected set; }
        
        /// <summary>
        /// The current acceleration of the TuioContainer. Can be calculated or given by the TUIO-Sender.
        /// </summary>
        public float Acceleration { get; protected set; }
        
        /// <summary>
        /// The current state of the TuioContainer.
        /// </summary>
        public TuioState State { get; protected set; }
        
        /// <summary>
        /// Is called every time the container gets updated.
        /// </summary>
        public event Action OnUpdate;
        
        /// <summary>
        /// Is called once when the state changes to removed.
        /// </summary>
        public event Action OnRemove;
        
        protected readonly List<Tuio11Point> PrevPoints = new List<Tuio11Point>();

        public Tuio11Container(TuioTime startTime, uint sessionId, Vector2 position, Vector2 velocity, float acceleration) : base(startTime, position)
        {
            CurrentTime = startTime;
            SessionId = sessionId;
            Velocity = velocity;
            Speed = Velocity.Length();
            PrevPoints.Add(new Tuio11Point(CurrentTime, position));
        }
        
        public List<Tuio11Point> GetPath()
        {
            return PrevPoints.ToList();
        }

        internal void UpdateTime(TuioTime currentTime)
        {
            CurrentTime = currentTime - StartTime;
        }

        internal void UpdateContainer(TuioTime currentTime, Vector2 position, Vector2 velocity, float acceleration, bool isCalculateSpeeds)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            Position = position;
            if (isCalculateSpeeds)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                var deltaPosition = Position - lastPoint.Position;
                var distance = deltaPosition.Length();
                var lastMotionSpeed = Speed;
                if(dt > 0)
                {
                    Velocity = deltaPosition / dt;
                    Speed = distance / dt;
                    Acceleration = (Speed - lastMotionSpeed) / dt;
                }
            }
            else
            {
                Velocity = velocity;
                Speed = Velocity.Length();
                Acceleration = acceleration;
            }

            PrevPoints.Add(new Tuio11Point(currentTime, position));
            if (PrevPoints.Count > MAX_PATH_LENGTH)
            {
                PrevPoints.RemoveAt(0);
            }

            if (Acceleration > 0)
            {
                State = TuioState.Accelerating;
            } 
            else if (Acceleration < 0)
            {
                State = TuioState.Decelerating;
            }
            else
            {
                State = TuioState.Stopped;
            }
            
            OnUpdate?.Invoke();
        }

        internal void Remove()
        {
            State = TuioState.Removed;
            OnRemove?.Invoke();
        }
    }
}