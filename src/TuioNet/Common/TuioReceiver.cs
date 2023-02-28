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
        public static TuioReceiver FromConnectionType(TuioConnectionType tuioConnectionType, string address="0.0.0.0", int port=0, bool isAutoProcess=true){
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
        public void ProcessMessages()
        {
            lock (_queuedMessages)
            {
                while (_queuedMessages.Count > 0)
                {
                    var oscMessage = _queuedMessages.Dequeue();
                    if (_messageListeners.TryGetValue(oscMessage.Address, out var messageListenersForAddress))
                    {
                        foreach (var messageListener in messageListenersForAddress)
                        {
                            messageListener.Invoke(oscMessage);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Adds a listener for a given TUIO profile.
        /// </summary>
        /// <param name="messageProfile">The TUIO profile to listen to.</param>
        /// <param name="listener">The callback method which gets invoked for the given adress.</param>
        /// <example>AddMessageListener("/tuio/2Dobj", OnCallback)</example>
        public void AddMessageListener(string messageProfile, Action<OSCMessage> listener)
        {
            if (!_messageListeners.ContainsKey(messageProfile))
            {
                _messageListeners[messageProfile] = new List<Action<OSCMessage>>();
            }
            _messageListeners[messageProfile].Add(listener);
        }
        
        /// <summary>
        /// Remove a listener from a given profile.
        /// </summary>
        /// <param name="messageProfile">The TUIO profile the listener should be removed from.</param>
        public void RemoveMessageListener(string messageProfile)
        {
            if (_messageListeners.ContainsKey(messageProfile))
            {
                _messageListeners.Remove(messageProfile);
            }
        }
    }
}
