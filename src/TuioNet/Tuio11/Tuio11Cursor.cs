using System.Numerics;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Tuio11
{
    public class Tuio11Cursor : Tuio11Container, ITuioEntity
    {
        /// <summary>
        /// Individual cursor ID assigned to each TuioCursor.
        /// </summary>
        public uint CursorId { get; }
        
        public Tuio11Cursor(TuioTime startTime, uint sessionId, uint cursorId, Vector2 position, Vector2 velocity, float acceleration) : base(startTime, sessionId, position, velocity, acceleration)
        {
            CursorId = cursorId;
        }
        
        public bool HasChanged(Vector2 position, Vector2 velocity, float acceleration)
        {
            return !(Position == position && Velocity == velocity && Acceleration == acceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position, Vector2 velocity, float acceleration)
        {
            UpdateContainer(currentTime, position, velocity, acceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position)
        {
            UpdateContainer(currentTime, position);
        }
        
        public OSCMessage OscMessage
        {
            get
            {
                var message = new OSCMessage("/tuio/2Dcur");
                message.Append("set");
                message.Append((int)SessionId);
                message.Append(Position.X);
                message.Append(Position.Y);
                message.Append(Velocity.X);
                message.Append(Velocity.Y);
                message.Append(Acceleration);
                return message;
            }
        }
    }
}