using System.Net.Sockets;

namespace TuioNet.Client.Common
{
    public class UdpTuioReceiver : TuioReceiver
    {
        private readonly int _port;

        internal UdpTuioReceiver(int port, bool isAutoProcess) : base(isAutoProcess)
        {
            _port = port;
        }
        
        /// <summary>
        /// Establish a connection to the TUIO sender over UDP.
        /// </summary>
        internal override void Connect()
        {
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            Task.Run(async () =>
            {
                using (var udpClient = new UdpClient(_port))
                {
                    IsConnected = true;
                    while (true)
                    {
                        try
                        {
                            var receivedResults = await udpClient.ReceiveAsync();
                            OnBuffer(receivedResults.Buffer, receivedResults.Buffer.Length);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            });
            IsConnected = false;
        }

        internal override void Disconnect()
        {
            CancellationTokenSource.Cancel();
        }
    }
}