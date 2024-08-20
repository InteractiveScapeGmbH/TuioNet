using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TuioNet.Common;

public class WebsocketClient
{
    private readonly CancellationTokenSource _tokenSource = new();
    private CancellationToken _token;
    private ClientWebSocket _socket;
    private readonly Uri _boxUrl;
    public bool IsConnected { get; private set; } = false;

    public event EventHandler<MessageEventArgs> OnMessageReceived;

    public WebsocketClient(string ipAddress, int port, CancellationTokenSource? tokenSource = null)
    {
        _boxUrl = new Uri($"ws://{ipAddress}:{port}");
        if(tokenSource != null)
            _tokenSource = tokenSource;
    }
    
    public void Connect()
    {
        _token = _tokenSource.Token;
        _token.ThrowIfCancellationRequested();
        Task.Run(async () =>
        {
            while (_socket is not { State: WebSocketState.Open })
            {
                if (_token.IsCancellationRequested) break;
                using(_socket = new ClientWebSocket())
                {
                    try
                    {
                        Console.WriteLine($"[WebsocketClient] Try to connect to: {_boxUrl}");
                        await _socket.ConnectAsync(_boxUrl, _token);
                        Console.WriteLine($"[WebsocketClient] Connected to {_boxUrl}.");
                        IsConnected = true;
                        await ReceiveMessages();
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"[WebsocketClient] There was a problem while receiving messages from: {_boxUrl}");
                        break;
                    }
                    catch (WebSocketException exception)
                    {
                        Console.WriteLine($"[WebsocketClient] Could not connect to {_boxUrl} -> {exception.Message}");
                        Console.WriteLine("[WebsocketClient] Try to establish connection again.");
                    }
                }
            }
            Console.WriteLine($"[WebsocketClient] Shutdown websocket to {_boxUrl}.");
            IsConnected = false;
        }, _token);
    }

    private async Task ReceiveMessages()
    {
        while (_socket.State == WebSocketState.Open)
        {
            var dataPerPacket = 4096;
            var buffer = new byte[dataPerPacket];
            var offset = 0;

            while (true)
            {
                try
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, dataPerPacket), _token);
                    offset += result.Count;
                    if (result.EndOfMessage) break;
                }
                catch (WebSocketException exception)
                {
                    Console.WriteLine($"[WebsocketClient] Could not receive message from {_boxUrl} -> {exception.Message}");
                    break;
                }
            }

            var messageArgs = new MessageEventArgs
            {
                Buffer = buffer,
                Length = offset
            };
            OnMessageReceived?.Invoke(this, messageArgs);
        }
    }

    public async void Send(string message)
    {
        if (_socket.State == WebSocketState.Open)
        {
            var decoded = Encoding.ASCII.GetBytes(message);
            var segment = new ArraySegment<byte>(decoded, 0, decoded.Length);
            await _socket.SendAsync(segment, WebSocketMessageType.Text, true, _tokenSource.Token);
        }
        else
        {
            Console.WriteLine("[WebsocketClient] Could not send message. Websocket is not connected.");
        }
    }

    public void Disconnect()
    {
        _tokenSource.Cancel();
    }
}