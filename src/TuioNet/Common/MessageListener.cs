using System;
using TuioNet.OSC;

namespace TuioNet.Common
{
    public struct MessageListener
    {
        public MessageListener(string messageProfile, EventHandler<OSCMessage> callback)
        {
            MessageProfile = messageProfile;
            Callback = callback;
        }
        public string MessageProfile { get; }
        public EventHandler<OSCMessage> Callback { get; }
    }
}