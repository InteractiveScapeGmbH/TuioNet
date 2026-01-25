using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using TuioNet.Server;

namespace TuioNet.ServerDemo;

public class TuioTransmitter
{
    private readonly ITuioServer _server;
    private readonly ITuioManager _manager;
    private bool _isInitialized;
    private readonly ILogger _logger;

    public TuioTransmitter(ITuioServer server, ITuioManager manager, ILogger logger)
    {
        _server = server;
        _manager = manager;
        _logger = logger;
        _isInitialized = false;
    }

    public void Open(IPAddress ipAddress, int port)
    {
        _server.Start(ipAddress, port);
        _isInitialized = true;
    }

    public void Send()
    {
        _manager.Update();
        try
        {
            foreach(var bundle in _manager.FrameBundles)
            {
                _logger.LogDebug(bundle.ToString());
                _server.Send(bundle.BinaryData);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError("Could not send Tuio message: {e}", exception);
        }
    }

    public void Close()
    {
        _isInitialized = false;
        _manager.Quit();
        try
        {
            foreach (var bundle in _manager.FrameBundles) 
            {
                _server.Send(bundle.BinaryData);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError("Could not send last data: {e}", exception);
        }
        _server.Stop();
    }
}