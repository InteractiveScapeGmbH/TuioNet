using System;
using System.Collections.Generic;
using System.Linq;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Component : Tuio20Point
    {
        private static int MAX_PATH_LENGTH = 128; 
        
        public TuioTime CurrentTime { get; protected set; }
        public Tuio20Object Container { get; protected set; }
        public float Angle { get; protected set; }
        public float xVel { get; protected set; }
        public float yVel { get; protected set; }
        public float mVel { get; protected set; }
        public float aVel { get; protected set; }
        public float mAcc { get; protected set; }
        public float rAcc { get; protected set; }
        public TuioState State { get; protected set; }
        public Queue<Tuio20Point> PrevPoints = new Queue<Tuio20Point>();

        public Tuio20Component(TuioTime startTime, Tuio20Object container, float xPos, float yPos, float angle,
            float xVel, float yVel, float aVel, float mAcc, float rAcc) : base(startTime, xPos, yPos)
        {
            CurrentTime = startTime;
            Container = container;
            this.xPos = xPos;
            this.yPos = yPos;
            Angle = angle;
            this.xVel = xVel;
            this.yVel = yVel;
            mVel = (float)Math.Sqrt(xVel * xVel + yVel * yVel);
            this.aVel = aVel;
            this.mAcc = mAcc;
            this.rAcc = rAcc;
            State = TuioState.Added;
            PrevPoints.Enqueue(new Tuio20Point(startTime, xPos, yPos));
        }
        
        public uint SessionId => Container.SessionId;
        
        public List<Tuio20Point> GetPath()
        {
            return PrevPoints.ToList();
        }

        public void UpdateComponent(TuioTime currentTime, float xPos, float yPos, float angle,
            float xVel, float yVel, float aVel, float mAcc, float rAcc)
        {
            CurrentTime = currentTime;
            this.xPos = xPos;
            this.yPos = yPos;
            Angle = angle;
            this.xVel = xVel;
            this.yVel = yVel;
            mVel = (float)Math.Sqrt(xVel * xVel + yVel * yVel);
            this.aVel = aVel;
            this.mAcc = mAcc;
            this.rAcc = rAcc;
            PrevPoints.Enqueue(new Tuio20Point(currentTime, xPos, yPos));
            if (PrevPoints.Count > MAX_PATH_LENGTH)
            {
                PrevPoints.Dequeue();
            }
            if (this.mAcc > 0)
            {
                State = TuioState.Accelerating;
            }
            else if (this.mAcc < 0)
            {
                State = TuioState.Decelerating;
            }
            else if (this.rAcc != 0 && State == TuioState.Stopped)
            {
                State = TuioState.Rotating;
            }
            else 
            {
                State = TuioState.Stopped;
            }
            Container.Update(currentTime);
        }
        
        internal void _remove(TuioTime currentTime)
        {
            CurrentTime = currentTime;
            State = TuioState.Removed;
        }
    }
}