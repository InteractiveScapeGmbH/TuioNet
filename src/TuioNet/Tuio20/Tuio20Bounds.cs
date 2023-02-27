using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Bounds: Tuio20Component
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Area { get; private set; }
        
        public Tuio20Bounds(TuioTime startTime, Tuio20Object container, float xPos, float yPos, float angle, float width, float height, float area, float xVel, float yVel, float aVel, float mAcc, float rAcc) : base(startTime, container, xPos, yPos, angle, xVel, yVel, aVel, mAcc, rAcc)
        {
            Width = width;
            Height = height;
            Area = area;
        }
        
        public bool HasChanged(float xPos, float yPos, float angle, float width, float height, float area, float xVel, float yVel, float aVel, float mAcc, float rAcc)
        {
            return !(xPos == this.xPos && yPos == this.yPos && angle == Angle && width == Width && height == Height && area == Area &&
                     xVel == base.xVel && yVel == base.yVel && aVel == base.aVel && mAcc == base.mAcc && rAcc == base.rAcc);
        }

        public void Update(TuioTime currentTime, float xPos, float yPos, float angle, float width, float height, float area, float xVel, float yVel, float aVel, float mAcc, float rAcc)
        {
            UpdateComponent(currentTime, xPos, yPos, angle, xVel, yVel, aVel, mAcc, rAcc);
            Width = width;
            Height = height;
            Area = area;
        }
    }
}