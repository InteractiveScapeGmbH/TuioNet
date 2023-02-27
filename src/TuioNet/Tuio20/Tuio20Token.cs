using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Token : Tuio20Component
    {
        public uint TuId { get; private set; }
        public uint CId { get; private set; }
        
        public Tuio20Token(TuioTime startTime, Tuio20Object container, uint tuId, uint cId, float xPos, float yPos, float angle, float xVel, float yVel, float aVel, float mAcc, float rAcc) : base(startTime, container, xPos, yPos, angle, xVel, yVel, aVel, mAcc, rAcc)
        {
            TuId = tuId;
            CId = cId;
        }
        
        public bool HasChanged(uint tuId, uint cId, float xPos, float yPos, float angle, float xVel, float yVel,
            float aVel, float mAcc, float rAcc)
        {
            return !(tuId == TuId && cId == CId && xPos == this.xPos && yPos == this.yPos && angle == Angle &&
                     xVel == base.xVel && yVel == base.yVel && aVel == base.aVel && mAcc == base.mAcc && rAcc == base.rAcc);
        }

        public void Update(TuioTime currentTime, uint tuId, uint cId, float xPos, float yPos, float angle,
            float xVel, float yVel,
            float aVel, float mAcc, float rAcc)
        {
            UpdateComponent(currentTime, xPos, yPos, angle, xVel, yVel, aVel, mAcc, rAcc);
            TuId = tuId;
            CId = cId;
        }
    }
}