using System;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TuioNet.Server;

public class WebsocketServer : ITuioServer
{
    private WebSocketServer _server;
    private readonly Action<string> _onLog;

    public WebsocketServer(Action<string> onLog)
    {
        _onLog = onLog;
    }
    
    public void Start(IPAddress address, int port)
    {
        _server = new WebSocketServer(address, port);
        _server.AddWebSocketService("/", CreateService);
        _server.AddWebSocketService<TuioService>("/");
        try
        {
            _server.Start();
        }
        catch (Exception exception)
        {
            throw;
        }
    }

    private TuioService CreateService()
    {
        var service = new TuioService();
        service.Init(_onLog);
        return service;
    }

    public void Stop()
    {
        _server.RemoveWebSocketService("/");
        _server.Stop();
    }

    public void Send(string data)
    {
        foreach (var host in _server.WebSocketServices.Hosts)
        {
            host.Sessions.Broadcast(data);
        }
    }

    public void Send(byte[] data)
    {
        foreach (var host in _server.WebSocketServices.Hosts)
        {
            host.Sessions.Broadcast(data);
        }
    }
}

public class TuioService : WebSocketBehavior
{
    private Action<string> OnLog;

    public void Init(Action<string> onLog)
    {
        OnLog = onLog;
    }
    
    protected override void OnOpen()
    {
        OnLog("[Server] New client connected.");
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        OnLog.Invoke($"[Server] New message from client: {e.Data}");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        OnLog.Invoke($"[Server] Client disconnected. {e.Reason}");
    }
}
