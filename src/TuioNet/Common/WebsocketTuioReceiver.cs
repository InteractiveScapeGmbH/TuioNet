namespace TuioNet.Common
{
    public class WebsocketTuioReceiver : TuioReceiver
    {
        private readonly WebsocketClient _client;

        public override bool IsConnected => _client.IsConnected;
        
        internal WebsocketTuioReceiver(string address, int port, bool isAutoProcess) : base(isAutoProcess)
        {
            _client = new WebsocketClient(address, port, CancellationTokenSource);
            _client.OnMessageReceived += MessageReceived;
        }

        private void MessageReceived(object? sender, MessageEventArgs message)
        {
            OnBuffer(message.Buffer, message.Length);
        }

        /// <summary>
        /// Establish a connection to the TUIO sender via Websocket.
        /// </summary>
        internal override void Connect()
        {
            _client.Connect();
        }
    }
}