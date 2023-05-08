using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public interface ITimeRefresher
    {
        public void Refresh(TuioTime tuioTime);
    }
}