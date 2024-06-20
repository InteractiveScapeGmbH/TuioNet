using System;
using Stride.Core.Mathematics;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Blob : Tuio11Container, IBoundsDrawer
    {
        /// <summary>
        /// The individual blob ID that is assigned to each TuioBlob.
        /// </summary>
        public uint BlobId { get; }
        
        /// <summary>
        /// The rotation angle ob the TuioBlob in radians.
        /// </summary>
        public float Angle { get; protected set; }

        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText => $"s_Id: {SessionId}\nAngle: {(Angle * 180f / Math.PI):f2}\nPosition: {Position:f2}Size: {Size:f2}\nTime: {CurrentTime.GetTotalMilliseconds()}";

        /// <summary>
        /// The normalized size of the TuioBlob. [0..1]
        /// </summary>
        public Vector2 Size { get; protected set; }
        
        /// <summary>
        /// 
        /// </summary>
        public float Area { get; protected set; }
        public float RotationSpeed { get; protected set; }
        public float RotationAcceleration { get; protected set; }
        
        public Tuio11Blob(TuioTime startTime, uint sessionId, uint blobId, Vector2 position, float angle, Vector2 size, float area, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration) : base(startTime, sessionId, position, velocity, acceleration)
        {
            BlobId = blobId;
            Angle = angle;
            Size = size;
            Area = area;
            RotationSpeed = rotationSpeed;
            RotationAcceleration = rotationAcceleration;
        }
        
        public bool HasChanged(Vector2 position, float angle, Vector2 size, float area,
           Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            return !(position.X == ((Tuio11Point)this).Position.X && position.Y == ((Tuio11Point)this).Position.Y && angle == Angle && size == Size && area == Area && velocity.X == base.Velocity.X && velocity.Y == base.Velocity.Y &&
                     rotationSpeed == RotationSpeed && acceleration == Acceleration && rotationAcceleration == RotationAcceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position, float angle, Vector2 size,
            float area, Vector2 velocity, float rotationSpeed, float acceleration, float rotationAcceleration)
        {
            var lastPoint = PrevPoints[PrevPoints.Count - 1];
            var isCalculateSpeeds = (position.X != ((Tuio11Point)this).Position.X && velocity.X == 0) || (position.Y != ((Tuio11Point)this).Position.Y && velocity.Y == 0);
            UpdateContainer(currentTime, position, velocity, acceleration, isCalculateSpeeds);

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
                    RotationAcceleration = (RotationSpeed - lastRotationSpeed) / dt;
                }
            }
            else
            {
                RotationSpeed = rotationSpeed;
                RotationAcceleration = rotationAcceleration;
            }
            Angle = angle;

            if (State != TuioState.Stopped && RotationAcceleration != 0)
            {
                State = TuioState.Rotating;
            }

            Size = size;
            Area = area;
        }
    }
}