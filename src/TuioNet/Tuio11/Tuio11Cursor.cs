﻿using System.Numerics;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Cursor : Tuio11Container, ITouchDrawer
    {
        /// <summary>
        /// Individual cursor ID assigned to each TuioCursor.
        /// </summary>
        public uint CursorId { get; }
        
        public Tuio11Cursor(TuioTime startTime, uint sessionId, uint cursorId, Vector2 position, Vector2 velocity, float acceleration) : base(startTime, sessionId, position, velocity, acceleration)
        {
            CursorId = cursorId;
        }
        
        internal bool HasChanged(Vector2 position, Vector2 velocity, float acceleration)
        {
            return !(Position == position && Velocity == velocity && Acceleration == acceleration);
        }

        internal void Update(TuioTime currentTime, Vector2 position, Vector2 velocity, float acceleration)
        {
            var isCalculateSpeeds = (position.X != ((Tuio11Point)this).Position.X && velocity.X == 0) || (position.Y != ((Tuio11Point)this).Position.Y && velocity.Y == 0);
            UpdateContainer(currentTime, position, velocity, acceleration, isCalculateSpeeds);
        }
        
        /// <summary>
        /// Returns a debug string with which one can display basic properties of the recognized TUIO object.
        /// </summary>
        public string DebugText =>
            $"Id: {SessionId}\nPosition: {Position:f2}";
    }
}