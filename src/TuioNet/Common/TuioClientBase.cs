namespace TuioNet.Common
{
    public abstract class TuioClientBase
    {
        /// <summary>
        /// Create new client for TUIO connection.
        /// </summary>
        /// <param name="connectionType">Type of the protocol which gets used to connect to the sender.</param>
        /// <param name="address">The IP address of the TUIO sender.</param>
        /// <param name="port">The port the client listen to for new TUIO messages. Default UDP port is 3333.</param>
        /// <param name="isAutoProcess">If set, the receiver processes incoming messages automatically. Otherwise the ProcessMessages() methods needs to be called manually.</param>
        public TuioClientBase(TuioConnectionType connectionType, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)
        {
            TuioReceiver = TuioReceiver.FromConnectionType(connectionType, address, port, isAutoProcess);
            AddMessageListeners();
        }

        protected readonly TuioReceiver TuioReceiver;
        protected TuioTime CurrentTime;

        /// <summary>
        /// Returns true if the receiver is connected to the TUIO sender.
        /// </summary>
        public bool IsConnected => TuioReceiver.IsConnected;
        
        /// <summary>
        /// Establish a connection to the TUIO sender.
        /// </summary>
        public void Connect()
        {
            TuioTime.Init();
            CurrentTime = TuioTime.GetCurrentTime();
            TuioReceiver.Connect();
        }

        /// <summary>
        /// Closes the connection to the TUIO sender.
        /// </summary>
        public void Disconnect()
        {
            TuioReceiver.Disconnect();
        }

        /// <summary>
        /// Process the TUIO messages in the message queue and invoke callbacks of the associated message listener. Only needs to be called if isAutoProcess is set to false.
        /// </summary>
        public void ProcessMessages()
        {
            TuioReceiver.ProcessMessages();
        }

        protected abstract void AddMessageListeners();
    }
}