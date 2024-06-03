using System;
using System.Collections.Generic;
using System.Threading;
using OSC.NET;

namespace TuioNet.Common
{
    public abstract class TuioReceiver
    {
        /// <summary>
        /// Returns true if the receiver is connected to the TUIO sender.
        /// </summary>
        public bool IsConnected { get; protected set; }
        
        private readonly Dictionary<string, List<Action<OSCMessage>>> _messageListeners = new Dictionary<string, List<Action<OSCMessage>>>();
        private readonly Queue<OSCMessage> _queuedMessages = new Queue<OSCMessage>();
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private readonly bool _isAutoProcess;

        protected TuioReceiver(bool isAutoProcess)
        {
            _isAutoProcess = isAutoProcess;
        }
        
        /// <summary>
        /// Returns a TuioReceiver object based on the given connection type.
        /// </summary>
        /// <param name="tuioConnectionType">The connection type which will be used to connect to the TUIO sender.</param>
        /// <param name="address">The IP address of the TUIO sender.</param>
        /// <param name="port">The port on which the receiver should listen to.</param>
        /// <param name="isAutoProcess">If true the TUIO messages gets processed automatically. No need for calling the ProcessMessages() method manually.</param>
        /// <returns>A TuioReceiver object. </returns>
        internal static TuioReceiver FromConnectionType(TuioConnectionType tuioConnectionType, string address, int port, bool isAutoProcess){
            switch(tuioConnectionType)
            {
                case TuioConnectionType.UDP:
                    return new UdpTuioReceiver(port, isAutoProcess);
                case TuioConnectionType.Websocket:
                    return new WebsocketTuioReceiver(address, port, isAutoProcess);
            }
            return null;
        }
        
        internal abstract void Connect();

        /// <summary>
        /// Close the connection to the TUIO sender.
        /// </summary>
        internal void Disconnect()
        {
            CancellationTokenSource.Cancel();
        }

        protected void OnBuffer(byte[] buffer, int end)
        {
            int start = 0;

            var packet = OSCPacket.Unpack(buffer, ref start, end);
            if (packet != null)
            {
                if (packet.IsBundle())
                {
                    packet.Values.ForEach(oscMessage =>
                    {
                        lock (_queuedMessages)
                        {
                            _queuedMessages.Enqueue((OSCMessage)oscMessage);
                        }
                    });
                }
                else
                {
                    lock (_queuedMessages)
                    {
                        _queuedMessages.Enqueue((OSCMessage)packet);
                    }
                }
            }

            if (_isAutoProcess)
            {
                ProcessMessages();
            }
        }
        
        /// <summary>
        /// Process the TUIO messages in the message queue and invoke callbacks of the associated message listener.
        /// </summary>
        internal void ProcessMessages()
        {
            lock (_queuedMessages)
            {
                while (_queuedMessages.Count > 0)
                {
                    var oscMessage = _queuedMessages.Dequeue();
                    if (!_messageListeners.TryGetValue(oscMessage.Address, out var messageListenersForAddress)) continue;
                    foreach (var messageListener in messageListenersForAddress)
                    {
                        messageListener.Invoke(oscMessage);
                    }
                }
            }
        }
        
        /// <summary>
        /// Adds a listener for a given TUIO profile.
        /// </summary>
        /// <param name="listener">The MessageListener which contains the name of the profile and the callback method which gets invoked for the given profile.</param>
        /// <example>
        /// <code>AddMessageListener("/tuio/2Dobj", OnCallback)</code>
        /// </example>
        internal void AddMessageListener(MessageListener listener)
        {
            var profile = listener.MessageProfile;
            if (!_messageListeners.ContainsKey(profile))
            {
                _messageListeners[profile] = new List<Action<OSCMessage>>();
            }
            _messageListeners[profile].Add(listener.Callback);
        }
        
        /// <summary>
        /// Remove a listener from a given profile.
        /// </summary>
        /// <param name="messageProfile">The TUIO profile the listener should be removed from.</param>
        internal void RemoveMessageListener(string messageProfile)
        {
            if (_messageListeners.ContainsKey(messageProfile))
            {
                _messageListeners.Remove(messageProfile);
            }
        }
    }
}
