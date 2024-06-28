using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Symbol : Tuio20Component
    {
        /// <summary>
        /// Allows multiplexing of various symbol types and association of additional user id. First two bytes encode user id. Last two bytes encode type id.
        /// User id 0 is reserved for "no user".
        /// </summary>
        public int TypeUserId { get; private set; }
        
        /// <summary>
        /// Allows distinction of individual symbols during a session.
        /// </summary>
        public int ComponentId { get; private set; }
        
        /// <summary>
        /// Describes the symbol type such as fiducial markers, barcodes or RFID tags.
        /// </summary>
        public string Group { get; private set; }
        
        /// <summary>
        /// The data which gets transmitted.
        /// </summary>
        public string Data { get; private set; }
        
        public Tuio20Symbol(TuioTime startTime, Tuio20Object container, int typeUserId, int componentId, string group, string data) : base(startTime, container, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0)
        {
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Group = group;
            Data = data;
        }
        
        internal bool HasChanged(int typeUserId, int componentId, string group, string data)
        {
            return !(typeUserId == TypeUserId && componentId == ComponentId && group == Group && data == Data);
        }

        internal void Update(TuioTime currentTime, int typeUserId, int componentId, string group, string data)
        {
            UpdateComponent(currentTime, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0);
            TypeUserId = typeUserId;
            ComponentId = componentId;
            Group = group;
            Data = data;
        }
    }
}