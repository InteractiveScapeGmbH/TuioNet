using System;
using System.Collections.Generic;
using System.Linq;
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
        /// The x-Component of the velocity vector.
        /// </summary>
        public float SpeedX { get; protected set; }
        
        /// <summary>
        /// The y-Component of the velocity vector.
        /// </summary>
        public float SpeedY { get; protected set; }
        
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

        public Tuio11Container(TuioTime startTime, uint sessionId, float posX, float posY, float speedX, float speedY, float motionAccel) : base(startTime, posX, posY)
        {
            CurrentTime = startTime;
            SessionId = sessionId;
            SpeedX = speedX;
            SpeedY = speedY;
            MotionSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            PrevPoints.Add(new Tuio11Point(CurrentTime, posX, posY));
        }
        
        public List<Tuio11Point> GetPath()
        {
            return PrevPoints.ToList();
        }

        internal void UpdateTime(TuioTime currentTime)
        {
            CurrentTime = currentTime - StartTime;
        }

        internal void UpdateContainer(TuioTime currentTime, float posX, float posY, float speedX, float speedY, float motionAccel, bool isCalculateSpeeds)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            PosX = posX;
            PosY = posY;
            if (isCalculateSpeeds)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                var dx = posX - lastPoint.PosX;
                var dy = posY - lastPoint.PosY;
                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                var lastMotionSpeed = MotionSpeed;
                if(dt > 0)
                {
                    this.SpeedX = dx / dt;
                    this.SpeedY = dy / dt;
                    MotionSpeed = dist / dt;
                    MotionAccel = (MotionSpeed - lastMotionSpeed) / dt;
                }
            }
            else
            {
                SpeedX = speedX;
                SpeedY = speedY;
                MotionSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                MotionAccel = motionAccel;
            }

            PrevPoints.Add(new Tuio11Point(currentTime, posX, posY));
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