using System;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Blob : Tuio11Container
    {
        /// <summary>
        /// The individual blob ID that is assigned to each TuioBlob.
        /// </summary>
        public uint BlobId { get; protected set; }
        
        /// <summary>
        /// The rotation angle ob the TuioBlob in radians.
        /// </summary>
        public float Angle { get; protected set; }
        
        /// <summary>
        /// The normalized width of the TuioBlob. [0..1]
        /// </summary>
        public float Width { get; protected set; }
        
        /// <summary>
        /// The normalized height of the TuioBlob. [0..1]
        /// </summary>
        public float Height { get; protected set; }
        
        /// <summary>
        /// 
        /// </summary>
        public float Area { get; protected set; }
        public float RotationSpeed { get; protected set; }
        public float RotationAccel { get; protected set; }
        
        public Tuio11Blob(TuioTime startTime, uint sessionId, uint blobId, float posX, float posY, float angle, float width, float height, float area, float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel) : base(startTime, sessionId, posX, posY, speedX, speedY, motionAccel)
        {
            BlobId = blobId;
            Angle = angle;
            Width = width;
            Height = height;
            Area = area;
            RotationSpeed = rotationSpeed;
            RotationAccel = rotationAccel;
        }
        
        public bool HasChanged(float posX, float posY, float angle, float width, float height, float area,
            float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            return !(posX == ((Tuio11Point)this).PosX && posY == ((Tuio11Point)this).PosY && angle == Angle && width == Width && height == Height && area == Area && speedX == base.SpeedX && speedY == base.SpeedY &&
                     rotationSpeed == RotationSpeed && motionAccel == MotionAccel && rotationAccel == RotationAccel);
        }

        public void Update(TuioTime currentTime, float posX, float posY, float angle, float width, float height,
            float area,
            float speedX, float speedY, float rotationSpeed, float motionAccel, float rotationAccel)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (posX != ((Tuio11Point)this).PosX && speedX == 0) || (posY != ((Tuio11Point)this).PosY && speedY == 0);
            UpdateContainer(currentTime, posX, posY, speedX, speedY, motionAccel, isCalculateSpeeds);

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