using System.Collections.Generic;

namespace TuioNet.Common
{
    public class TuioClient
    {
        /// <summary>
        /// Create new client for TUIO connection.
        /// </summary>
        /// <param name="connectionType">Type of the protocol which gets used to connect to the sender.</param>
        /// <param name="address">The IP address of the TUIO sender.</param>
        /// <param name="port">The port the client listen to for new TUIO messages. Default UDP port is 3333.</param>
        /// <param name="isAutoProcess">If set, the receiver processes incoming messages automatically. Otherwise the ProcessMessages() methods needs to be called manually.</param>
        public TuioClient(TuioConnectionType connectionType, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)
        {
            _tuioReceiver = TuioReceiver.FromConnectionType(connectionType, address, port, isAutoProcess);
        }

        private readonly TuioReceiver _tuioReceiver;

        /// <summary>
        /// Returns true if the receiver is connected to the TUIO sender.
        /// </summary>
        public bool IsConnected => _tuioReceiver.IsConnected;
        
        /// <summary>
        /// Establish a connection to the TUIO sender.
        /// </summary>
        public void Connect()
        {
            _tuioReceiver.Connect();
        }

        /// <summary>
        /// Closes the connection to the TUIO sender.
        /// </summary>
        public void Disconnect()
        {
            _tuioReceiver.Disconnect();
        }

        /// <summary>
        /// Process the TUIO messages in the message queue and invoke callbacks of the associated message listener. Only needs to be called if isAutoProcess is set to false.
        /// </summary>
        public void ProcessMessages()
        {
            _tuioReceiver.ProcessMessages();
        }

        public void AddMessageListeners(List<MessageListener> listeners)
        {
            foreach (var listener in listeners)
            {
                _tuioReceiver.AddMessageListener(listener.MessageProfile, listener.Callback);
            }
        }
    }
}