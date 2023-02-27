using System;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Blob : Tuio11Container
    {
        public uint BlobId { get; protected set; }
        public float Angle { get; protected set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public float Area { get; protected set; }
        public float RotationSpeed { get; protected set; }
        public float RotationAccel { get; protected set; }
        
        public Tuio11Blob(TuioTime startTime, uint sessionId, uint blobId, float xPos, float yPos, float angle, float width, float height, float area, float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel) : base(startTime, sessionId, xPos, yPos, xSpeed, ySpeed, motionAccel)
        {
            BlobId = blobId;
            Angle = angle;
            Width = width;
            Height = height;
            Area = area;
            RotationSpeed = rotationSpeed;
            RotationAccel = rotationAccel;
        }
        
        public bool HasChanged(float xPos, float yPos, float angle, float width, float height, float area,
            float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            return !(xPos == ((Tuio11Point)this).xPos && yPos == ((Tuio11Point)this).yPos && angle == Angle && width == Width && height == Height && area == Area && xSpeed == base.xSpeed && ySpeed == base.ySpeed &&
                     rotationSpeed == RotationSpeed && motionAccel == MotionAccel && rotationAccel == RotationAccel);
        }

        public void Update(TuioTime currentTime, float xPos, float yPos, float angle, float width, float height,
            float area,
            float xSpeed, float ySpeed, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (xPos != ((Tuio11Point)this).xPos && xSpeed == 0) || (yPos != ((Tuio11Point)this).yPos && ySpeed == 0);
            UpdateContainer(currentTime, xPos, yPos, xSpeed, ySpeed, motionAccel, isCalculateSpeeds);

            var isCalculateRotation = angle != Angle && rotationSpeed == 0;
            if(isCalculateRotation)
            {
                var dt = (currentTime - lastPoint.StartTime).GetTotalMilliseconds() / 1000.0f;
                if (dt > 0)
                {
                    var lastAngle = Angle;
                    var lastRotationSpeed = RotationSpeed;
                    var da = (angle - lastAngle) / (2 * (float)Math.PI);
                    if (da > 0.5f)
                    {
                        da -= 1;
                    } 
                    else if (da <= -0.5f)
                    {
                        da += 1;
                    }
                    RotationSpeed = da / dt;
                    RotationAccel = (RotationSpeed - lastRotationSpeed) / dt;
                }
            }
            else
            {
                RotationSpeed = rotationSpeed;
                RotationAccel = rotationAccel;
            }
            Angle = angle;

            if (State != TuioState.Stopped && RotationAccel != 0)
            {
                State = TuioState.Rotating;
            }

            Width = width;
            Height = height;
            Area = area;
        }
    }
}