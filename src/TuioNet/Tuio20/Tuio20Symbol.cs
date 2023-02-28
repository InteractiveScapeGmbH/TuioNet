using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Symbol : Tuio20Component
    {
        public uint TypeUserId { get; private set; }
        public uint ComponentId { get; private set; }
        public string Group { get; private set; }
        public string Data { get; private set; }
        
        public Tuio20Symbol(TuioTime startTime, Tuio20Object container, uint typeUserId, uint componentId, string group, string data) : base(startTime, container, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0)
        {
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Group = group;
            Data = data;
        }
        
        internal bool HasChanged(uint typeUserId, uint componentId, string group, string data)
        {
            return !(typeUserId == TypeUserId && componentId == ComponentId && group == Group && data == Data);
        }

        internal void Update(TuioTime currentTime, uint typeUserId, uint componentId, string group, string data)
        {
            UpdateComponent(currentTime, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0);
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Group = group;
            Data = data;
        }
    }
}