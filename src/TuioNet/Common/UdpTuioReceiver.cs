using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TuioNet.Common
{
    public class UdpTuioReceiver : TuioReceiver
    {
        private readonly int _port;

        public UdpTuioReceiver(int port, bool isAutoProcess) : base(isAutoProcess)
        {
            _port = port;
        }
        
        /// <summary>
        /// Establish a connection to the TUIO sender over UDP.
        /// </summary>
        public override void Connect()
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
    }
}