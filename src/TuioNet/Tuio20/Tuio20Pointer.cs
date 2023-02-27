using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Pointer : Tuio20Component
    {
        public uint TuId { get; private set; }
        public uint CId { get; private set; }
        public float Shear { get; private set; }
        public float Radius { get; private set; }
        public float Press { get; private set; }
        public float pVel { get; private set; }
        public float pAcc { get; private set; }
        
        public Tuio20Pointer(TuioTime startTime, Tuio20Object container, uint tuId, uint cId, float xPos, float yPos, float angle, float shear, float radius, float press, float xVel, float yVel, float pVel, float mAcc, float pAcc) : base(startTime, container, xPos, yPos, angle, xVel, yVel, 0, mAcc, 0)
        {
            TuId = tuId;
            CId = cId;
            Shear = shear;
            Radius = radius;
            Press = press;
            this.pVel = pVel;
            this.pAcc = pAcc;
        }
        
        internal bool HasChanged(uint tuId, uint cId, float xPos, float yPos, float angle, float shear, float radius, float press, float xVel, float yVel, float pVel, float mAcc, float pAcc)
        {
            return !(tuId == TuId && cId == CId && xPos == this.xPos && yPos == this.yPos && angle == Angle && shear == Shear &&radius == Radius &&press == Press &&
                     xVel == base.xVel && yVel == base.yVel && pVel == this.pVel && mAcc == base.mAcc && pAcc == this.pAcc);
        }

        internal void Update(TuioTime currentTime, uint tuId, uint cId, float xPos, float yPos, float angle, float shear, float radius, float press, float xVel, float yVel, float pVel, float mAcc, float pAcc)
        {
            UpdateComponent(currentTime, xPos, yPos, angle, xVel, yVel, 0, mAcc, 0);
            TuId = tuId;
            CId = cId;
            Shear = shear;
            Radius = radius;
            Press = press;
            this.pVel = pVel;
            this.pAcc = pAcc;
        }
    }
}