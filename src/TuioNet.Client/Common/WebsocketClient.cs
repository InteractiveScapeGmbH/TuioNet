using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using TuioNet.Common;

namespace TuioNet.Client.Common;

public class WebsocketClient
{
    private readonly CancellationTokenSource _tokenSource = new();
    private CancellationToken _token;
    private ClientWebSocket _socket;
    private readonly Uri _boxUrl;
    public bool IsConnected { get; private set; } = false;

    public event EventHandler<MessageEventArgs> OnMessageReceived;

    private readonly ILogger _logger;

    public WebsocketClient(string ipAddress, int port, ILogger logger, CancellationTokenSource? tokenSource = null)
    {
        _logger = logger;
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
                        _logger.LogInformation($"[WebsocketClient] Try to connect to: {_boxUrl}");
                        await _socket.ConnectAsync(_boxUrl, _token);
                        _logger.LogInformation($"[WebsocketClient] Connected to {_boxUrl}.");
                        IsConnected = true;
                        await ReceiveMessages();
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogError($"[WebsocketClient] There was a problem while receiving messages from: {_boxUrl}");
                        break;
                    }
                    catch (WebSocketException exception)
                    {
                        _logger.LogError($"[WebsocketClient] Could not connect to {_boxUrl} -> {exception.Message}");
                        _logger.LogError("[WebsocketClient] Try to establish connection again.");
                    }
                }
            }
            _logger.LogInformation($"[WebsocketClient] Shutdown websocket to {_boxUrl}.");
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
                    _logger.LogError($"[WebsocketClient] Could not receive message from {_boxUrl} -> {exception.Message}");
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
            _logger.LogError("[WebsocketClient] Could not send message. Websocket is not connected.");
        }
    }

    public void Disconnect()
    {
        _tokenSource.Cancel();
    }
}