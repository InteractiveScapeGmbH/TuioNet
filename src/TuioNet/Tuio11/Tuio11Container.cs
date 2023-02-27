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
        /// Time since creation as TuioTime.
        /// </summary>
        public TuioTime CurrentTime { get; protected set; }
        
        /// <summary>
        /// The Session ID, a temporary object ID.
        /// </summary>
        public uint SessionId { get; protected set; }
        
        /// <summary>
        /// The velocity-vector of the TuioContainer.
        /// </summary>
        public Vector2 Velocity { get; protected set; }
        
        /// <summary>
        /// Calculated length of the velocity vector.
        /// </summary>
        public float MotionSpeed { get; protected set; }
        
        /// <summary>
        /// The current acceleration of the TUIO Container. Can be calculated or given by the TUIO-Sender.
        /// </summary>
        public float MotionAccel { get; protected set; }
        
        /// <summary>
        /// The current state of the TUIO Container.
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

        public Tuio11Container(TuioTime startTime, uint sessionId, Vector2 position, Vector2 velocity, float motionAccel) : base(startTime, position)
        {
            CurrentTime = startTime;
            SessionId = sessionId;
            Velocity = velocity;
            MotionSpeed = Velocity.Length();
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

        internal void UpdateContainer(TuioTime currentTime, Vector2 position, Vector2 velocity, float motionAccel, bool isCalculateSpeeds)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            Position = position;
            if (isCalculateSpeeds)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                var deltaPosition = Position - lastPoint.Position;
                var distance = deltaPosition.Length();
                var lastMotionSpeed = MotionSpeed;
                if(dt > 0)
                {
                    Velocity = deltaPosition / dt;
                    MotionSpeed = distance / dt;
                    MotionAccel = (MotionSpeed - lastMotionSpeed) / dt;
                }
            }
            else
            {
                Velocity = velocity;
                MotionSpeed = Velocity.Length();
                MotionAccel = motionAccel;
            }

            PrevPoints.Add(new Tuio11Point(currentTime, position));
            if (PrevPoints.Count > MAX_PATH_LENGTH)
            {
                PrevPoints.RemoveAt(0);
            }

            if (MotionAccel > 0)
            {
                State = TuioState.Accelerating;
            } 
            else if (MotionAccel < 0)
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