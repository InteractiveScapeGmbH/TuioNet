using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TuioNet.Common
{
    public class WebsocketTuioReceiver : TuioReceiver
    {
        private readonly string _address;
        private readonly int _port;

        public WebsocketTuioReceiver(string address, int port, bool isAutoProcess) : base(isAutoProcess)
        {
            _address = address;
            _port = port;
        }
        
        /// <summary>
        /// Establish a connection to the TUIO sender via Websocket.
        /// </summary>
        internal override void Connect()
        {
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            Task.Run(async () =>
            {
                Uri serverUri = new Uri("ws://" + _address + ":" + _port);
                using (var socket = new ClientWebSocket())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        await socket.ConnectAsync(serverUri, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    if (socket.State == WebSocketState.Open)
                    {
                        IsConnected = true;
                    }
                    while (socket.State == WebSocketState.Open)
                    {
                        var dataPerPacket = 4096;
                        var buffer = new byte[dataPerPacket];
                        var offset = 0;

                        while (true)
                        {
                            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(buffer, offset, dataPerPacket);
                            try
                            {
                                WebSocketReceiveResult result =
                                    await socket.ReceiveAsync(bytesReceived, cancellationToken);
                                offset += result.Count;
                                if (result.EndOfMessage)
                                    break;
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }

                        OnBuffer(buffer, offset);
                    }
                }
                IsConnected = false;
            });
        }
    }
}