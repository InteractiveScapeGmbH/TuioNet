using System;
using System.Net;
using Microsoft.Extensions.Logging;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TuioNet.Server
{
    public class WebsocketServer : ITuioServer
    {
        private readonly ILogger _logger;
        private WebSocketServer _server;
    
        public WebsocketServer(ILogger logger)
        {
            _logger = logger;
        }
    
        public void Start(IPAddress address, int port)
        {
            _server = new WebSocketServer(address, port);
            _server.AddWebSocketService("/", CreateService);
            // _server.AddWebSocketService<TuioService>("/");
            _server.Start();
        }
    
        public void Stop()
        {
            _server.RemoveWebSocketService("/");
            _server.Stop();
        }
    
        public void Send(string data)
        {
            foreach (var host in _server.WebSocketServices.Hosts) host.Sessions.Broadcast(data);
        }
    
        public void Send(byte[] data)
        {
            foreach (var host in _server.WebSocketServices.Hosts) host.Sessions.Broadcast(data);
        }
    
        private TuioService CreateService()
        {
            var service = new TuioService();
            service.Init(_logger);
            return service;
        }
    }
    
    public class TuioService : WebSocketBehavior
    {
        private ILogger? _logger;
    
        public void Init(ILogger logger)
        {
            _logger = logger;
        }
    
        protected override void OnOpen()
        {
            _logger?.LogInformation("[Server] New client connected.");
        }
    
        protected override void OnMessage(MessageEventArgs e)
        {
            _logger?.LogDebug("[Server] New message from client: {event_data}", e.Data);
        }
    
        protected override void OnClose(CloseEventArgs e)
        {
            _logger?.LogInformation("[Server] Client disconnected. {event_reason}", e.Reason);
        }
    }
}

