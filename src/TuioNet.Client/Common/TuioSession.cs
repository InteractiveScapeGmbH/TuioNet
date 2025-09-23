using Microsoft.Extensions.Logging;
using TuioNet.Client.Tuio11;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Client.Common;

public class TuioSession : IDisposable
{
    public TuioVersion TuioVersion { get; }
    public TuioConnectionType ConnectionType { get; }
    public string IpAddress { get; }
    public int Port { get; }

    public ITuioDispatcher TuioDispatcher { get; }

    private readonly ILogger _logger;
    private TuioClient _tuioClient;
    private bool _isInitialized;

    public TuioSession(ILogger logger, TuioVersion tuioVersion = TuioVersion.Tuio11,
        TuioConnectionType connectionType = TuioConnectionType.UDP, string ipAddress = "127.0.0.1", int port = 3333, bool isAutoProcess = true)
    {
        _logger = logger;
        TuioVersion = tuioVersion;
        ConnectionType = connectionType;
        IpAddress = ipAddress;
        Port = port;

        TuioDispatcher = TuioVersion switch
        {
            TuioVersion.Tuio11 => new Tuio11Dispatcher(),
            TuioVersion.Tuio20 => new Tuio20Dispatcher()
        };

        Initialize(isAutoProcess);
        logger.LogInformation("[TuioSession] Session initialized!");
    }

    private void Initialize(bool isAutoProcess)
    {
        if (_isInitialized) return;
        _tuioClient = new TuioClient(ConnectionType, _logger, IpAddress, Port, isAutoProcess);
        TuioDispatcher.SetupProcessor(_tuioClient);
        TuioDispatcher.RegisterCallbacks();
        _tuioClient.Connect();
        _isInitialized = true;
    }

    public void AddMessageListener(MessageListener listener)
    {
        _tuioClient.AddMessageListener(listener);
    }

    public void RemoveMessageListener(string listener)
    {
        _tuioClient.RemoveMessageListener(listener);
    }
    
    public void ProcessMessages()
    {
        _tuioClient.ProcessMessages();
    }

    public void Dispose()
    {
        TuioDispatcher.UnregisterCallbacks();
        _tuioClient.Disconnect();
        _isInitialized = false;
    }
}