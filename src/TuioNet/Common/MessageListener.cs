using System;
using OSC.NET;

namespace TuioNet.Common
{
    public struct MessageListener
    {
        public MessageListener(string messageProfile, Action<OSCMessage> callback)
        {
            MessageProfile = messageProfile;
            Callback = callback;
        }
        public string MessageProfile { get; }
        public Action<OSCMessage> Callback { get; }
    }
}