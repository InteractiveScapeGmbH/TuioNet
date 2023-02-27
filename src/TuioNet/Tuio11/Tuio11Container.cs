using System;
using System.Collections.Generic;
using System.Linq;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Container : Tuio11Point
    {
        private const int MAX_PATH_LENGTH = 128;

        public TuioTime CurrentTime { get; protected set; }
        public uint SessionId { get; protected set; }
        public float xSpeed { get; protected set; }
        public float ySpeed { get; protected set; }
        public float MotionSpeed { get; protected set; }
        public float MotionAccel { get; protected set; }
        public TuioState State { get; protected set; }

        public event Action OnUpdate;
        public event Action OnRemove;
        
        protected readonly List<Tuio11Point> PrevPoints = new List<Tuio11Point>();

        public Tuio11Container(TuioTime startTime, uint sessionId, float xPos, float yPos, float xSpeed, float ySpeed, float motionAccel) : base(startTime, xPos, yPos)
        {
            CurrentTime = startTime;
            SessionId = sessionId;
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            MotionSpeed = (float)Math.Sqrt(xSpeed * xSpeed + ySpeed * ySpeed);
            PrevPoints.Add(new Tuio11Point(CurrentTime, xPos, yPos));
        }
        
        public List<Tuio11Point> GetPath()
        {
            return PrevPoints.ToList();
        }

        internal void UpdateTime(TuioTime currentTime)
        {
            CurrentTime = currentTime - StartTime;
        }

        internal void UpdateContainer(TuioTime currentTime, float xPos, float yPos, float xSpeed, float ySpeed, float motionAccel, bool isCalculateSpeeds)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            base.xPos = xPos;
            base.yPos = yPos;
            if (isCalculateSpeeds)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                var dx = xPos - lastPoint.xPos;
                var dy = yPos - lastPoint.yPos;
                var dist = (float)Math.Sqrt(dx * dx + dy * dy);
                var lastMotionSpeed = MotionSpeed;
                if(dt > 0)
                {
                    this.xSpeed = dx / dt;
                    this.ySpeed = dy / dt;
                    MotionSpeed = dist / dt;
                    MotionAccel = (MotionSpeed - lastMotionSpeed) / dt;
                }
            }
            else
            {
                this.xSpeed = xSpeed;
                this.ySpeed = ySpeed;
                MotionSpeed = (float)Math.Sqrt(xSpeed * xSpeed + ySpeed * ySpeed);
                MotionAccel = motionAccel;
            }

            PrevPoints.Add(new Tuio11Point(currentTime, xPos, yPos));
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