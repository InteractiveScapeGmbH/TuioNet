using System;
using System.Collections.Generic;
using System.Threading;
using OSC.NET;

namespace Tuio.Common
{
    public abstract class TuioReceiver
    {
        internal bool _isConnected;
        private Dictionary<string, List<Action<OSCMessage>>> _messageListeners = new Dictionary<string, List<Action<OSCMessage>>>();
        private Queue<OSCMessage> _queuedMessages = new Queue<OSCMessage>();
        protected CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public abstract void Connect();

        public void Disconnect()
        {
            _cancellationTokenSource.Cancel();
        }
        
        public void OnBuffer(byte[] buffer, int end)
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
        }
        
        public void ProcessMessages()
        {
            lock (_queuedMessages)
            {
                while (_queuedMessages.Count > 0)
                {
                    OnOscMessage(_queuedMessages.Dequeue());
                }
            }
        }
        
        private void OnOscMessage(OSCMessage oscMessage)
        {
            if (_messageListeners.TryGetValue(oscMessage.Address, out var messageListenersForAddress))
            {
                foreach (var messageListener in messageListenersForAddress)
                {
                    messageListener.Invoke(oscMessage);
                }
            }
        }

        public void AddMessageListener(string address, Action<OSCMessage> listener)
        {
            if (!_messageListeners.ContainsKey(address))
            {
                _messageListeners[address] = new List<Action<OSCMessage>>();
            }
            _messageListeners[address].Add(listener);
        }
    }
}