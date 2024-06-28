﻿using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Cursor : Tuio11Container, ITuio11Entity, ITouchDrawer
    {
        /// <summary>
        /// Individual cursor ID assigned to each TuioCursor.
        /// </summary>
        public int CursorId { get; }
        
        public Tuio11Cursor(TuioTime startTime, int sessionId, int cursorId, Vector2 position, Vector2 velocity, float acceleration) : base(startTime, sessionId, position, velocity, acceleration)
        {
            CursorId = cursorId;
        }
        
        internal bool HasChanged(Vector2 position, Vector2 velocity, float acceleration)
        {
            return !(Position == position && Velocity == velocity && Acceleration == acceleration);
        }

        public void Update(TuioTime currentTime, Vector2 position, Vector2 velocity, float acceleration)
        {
            var isCalculateSpeeds = (position.X != ((Tuio11Point)this).Position.X && velocity.X == 0) || (position.Y != ((Tuio11Point)this).Position.Y && velocity.Y == 0);
            UpdateContainer(currentTime, position, velocity, acceleration, isCalculateSpeeds);
        }
        
        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText =>
            $"Id: {SessionId}\nPosition: {Position:f2}";
        
        public OSCMessage SetMessage
        {
            get
            {
                var message = new OSCMessage("/tuio/2Dcur");
                message.Append("set");
                message.Append(SessionId);
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