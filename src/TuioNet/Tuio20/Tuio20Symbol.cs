using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Symbol : Tuio20Component
    {
        public uint TuId { get; private set; }
        public uint CId { get; private set; }
        public string Group { get; private set; }
        public string Data { get; private set; }
        
        public Tuio20Symbol(TuioTime startTime, Tuio20Object container, uint tuId, uint cId, string group, string data) : base(startTime, container, 0, 0, 0, 0, 0, 0, 0, 0)
        {
            TuId = tuId;
            CId = cId;
            Group = group;
            Data = data;
        }
        
        internal bool HasChanged(uint tuId, uint cId, string group, string data)
        {
            return !(tuId == TuId && cId == CId && group == Group && data == Data);
        }

        internal void Update(TuioTime currentTime, uint tuId, uint cId, string group, string data)
        {
            UpdateComponent(currentTime, 0, 0, 0, 0, 0, 0, 0, 0);
            TuId = tuId;
            CId = cId;
            Group = group;
            Data = data;
        }
    }
}