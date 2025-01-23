using System.Collections.Generic;
using Microsoft.Extensions.Logging;

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
        public TuioClient(TuioConnectionType connectionType,ILogger logger, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)
        {
            _tuioReceiver = TuioReceiver.FromConnectionType(connectionType, address, port, isAutoProcess, logger);
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

        internal void AddMessageListeners(List<MessageListener> listeners)
        {
            foreach (var listener in listeners)
            {
                AddMessageListener(listener);
            }
        }

        /// <summary>
        /// Adds a listener to react on a custom profile.
        /// </summary>
        /// <param name="listener">The MessageListener which contains the name of the profile and the callback method which gets invoked for the given profile.</param>
        public void AddMessageListener(MessageListener listener)
        {
            _tuioReceiver.AddMessageListener(listener);
        }

        /// <summary>
        /// Remove a listener from a given profile.
        /// </summary>
        /// <param name="messageProfile">The TUIO profile the listener should be removed from.</param>
        public void RemoveMessageListener(string messageProfile)
        {
            _tuioReceiver.RemoveMessageListener(messageProfile);
        }
    }
}