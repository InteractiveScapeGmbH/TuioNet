using CommandLine;
using Microsoft.Extensions.Logging;
using TuioNet.Client.Common;
using TuioNet.Client.Tuio11;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.ClientDemo;

class Program
{
    public class Options
    {
        [Option('i', "ip", HelpText = "Set the ip address of the tuio sender.", Default = "127.0.0.1")]
        public string IpAddress { get; set; }
        [Option('p', "port", HelpText = "Set the port.", Default = 3333)]
        public int Port { get; set; }
        [Option('l', "logLevel", HelpText = "Set the minimum log level [Trace, Debug, Information, Warning, Error, Critical, None].", Default = LogLevel.Information)]
        public LogLevel LogLevel { get; set; }
        [Option('v', "tuioVersion", HelpText = "Set the tuio version [Tuio11, Tuio20].", Default = TuioVersion.Tuio11)]
        public TuioVersion TuioVersion { get; set; }
        [Option('c', "connection", HelpText = "Set the connection type [UDP, Websocket].", Default = TuioConnectionType.UDP)]
        public TuioConnectionType ConnectionType { get; set; }
    }
    
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
            using var tuioSession = new TuioSession(logger, option.TuioVersion, option.ConnectionType, option.IpAddress, option.Port);
            AddCallbacks(tuioSession);
            Console.WriteLine("Connect...");
            while (true)
            {
                if (!Console.KeyAvailable) continue;
                var pressedKey = Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.Q) break;
            }
            Console.WriteLine("Disconnect...");
            RemoveCallbacks(tuioSession);
        });
    }

    private static void AddCallbacks(TuioSession tuioSession)
    {
        switch (tuioSession.TuioVersion)
        {
            case TuioVersion.Tuio11:
                var dispatcher11 = (Tuio11Dispatcher)tuioSession.TuioDispatcher;
                dispatcher11.OnCursorAdd += CursorAdded;
                dispatcher11.OnCursorUpdate += UpdateCursor;
                dispatcher11.OnCursorRemove += RemoveCursor;
                
                dispatcher11.OnObjectAdd += ObjectAdded;
                dispatcher11.OnObjectUpdate += UpdateObject;
                dispatcher11.OnObjectRemove += RemoveObject;
                break;
            case TuioVersion.Tuio20:
                var dispatcher20 = (Tuio20Dispatcher)tuioSession.TuioDispatcher;
                dispatcher20.OnObjectAdd += Tuio20Add;
                dispatcher20.OnObjectUpdate += Tuio20Update;
                dispatcher20.OnObjectRemove += Tuio20Remove;
                break;
        }
    }
    
    private static void RemoveCallbacks(TuioSession tuioSession)
    {
        switch (tuioSession.TuioVersion)
        {
            case TuioVersion.Tuio11:
                var dispatcher11 = (Tuio11Dispatcher)tuioSession.TuioDispatcher;
                dispatcher11.OnCursorAdd -= CursorAdded;
                dispatcher11.OnCursorUpdate -= UpdateCursor;
                dispatcher11.OnCursorRemove -= RemoveCursor;
                
                dispatcher11.OnObjectAdd -= ObjectAdded;
                dispatcher11.OnObjectUpdate -= UpdateObject;
                dispatcher11.OnObjectRemove -= RemoveObject;
                break;
            case TuioVersion.Tuio20:
                var dispatcher20 = (Tuio20Dispatcher)tuioSession.TuioDispatcher;
                dispatcher20.OnObjectAdd -= Tuio20Add;
                dispatcher20.OnObjectUpdate -= Tuio20Update;
                dispatcher20.OnObjectRemove -= Tuio20Remove;
                break;
                
        }
    }
    
    private static void Tuio20Add(object? sender, Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"[Tuio 2.0] Pointer added -> SessionId: {tuio20Object.SessionId}");
        }

        if (tuio20Object.ContainsTuioToken())
        {
            Console.WriteLine($"[Tuio 2.0] Token added -> SessionId: {tuio20Object.SessionId}, ComponentId: {tuio20Object.Token.ComponentId}");
        }
    }

    private static void Tuio20Remove(object? sender, Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"[Tuio 2.0] Pointer removed -> SessionId: {tuio20Object.SessionId}");
        }

        if (tuio20Object.ContainsTuioToken())
        {
            Console.WriteLine($"[Tuio 2.0] Token removed -> SessionId: {tuio20Object.SessionId}, ComponentId: {tuio20Object.Token.ComponentId}");
        }
    }

    private static void Tuio20Update(object? sender, Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"[Tuio 2.0] Pointer {tuio20Object.SessionId} -> Position: {tuio20Object.Pointer.Position}");
        }

        if (tuio20Object.ContainsTuioToken())
        {
            Console.WriteLine($"[Tuio 2.0] Token {tuio20Object.Token.ComponentId} -> Position: {tuio20Object.Token.Position}, Angle: {tuio20Object.Token.Angle}");

        }
    }

    private static void ObjectAdded(object? sender, Tuio11Object tuio11Object)
    {
        Console.WriteLine($"[Tuio 1.1] Object added -> SessionId: {tuio11Object.SessionId}, SymbolId: {tuio11Object.SymbolId}");
    }

    private static void UpdateObject(object? sender, Tuio11Object tuio11Object)
    {
        Console.WriteLine($"[Tuio 1.1] Object {tuio11Object.SymbolId} -> Position: {tuio11Object.Position}");
    }

    private static void RemoveObject(object? sender, Tuio11Object tuio11Object)
    {
        Console.WriteLine($"[Tuio 1.1] Object removed -> SessionId: {tuio11Object.SessionId}, SymbolId: {tuio11Object.SymbolId}");
    }
    private static void CursorAdded(object? sender, Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"[Tuio 1.1] Cursor added -> SessionId: {tuio11Cursor.SessionId}");
    }

    private static void UpdateCursor(object? sender, Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"[Tuio 1.1] Cursor {tuio11Cursor.SessionId} -> Position: {tuio11Cursor.Position}");
    }
    
    private static void RemoveCursor(object? sender, Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"[Tuio 1.1] Cursor removed -> SessionId: {tuio11Cursor.SessionId}");
    }

}

