using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Demo;

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

    private static void Tuio20Remove(object? sender, Tuio20Object e)
    {
        Console.WriteLine($"Object {e.SessionId} removed");
    }

    private static void Tuio20Update(object? sender, Tuio20Object e)
    {
        if (e.ContainsTuioPointer())
        {
            Console.WriteLine($"Pointer {e.SessionId} -> Position: {e.Pointer.Position}");
        }

        if (e.ContainsTuioToken())
        {
            Console.WriteLine($"Pointer {e.Token.ComponentId} -> Position: {e.Token.Position}, Angle: {e.Token.Angle}");

        }
    }

    private static void Tuio20Add(object? sender, Tuio20Object e)
    {
        Console.WriteLine($"New object added -> ID: {e.SessionId}");
    }

    private static void RemoveObject(object? sender, Tuio11Object e)
    {
        Console.WriteLine($"Object {e.SymbolId} removed");
    }

    private static void UpdateObject(object? sender, Tuio11Object e)
    {
        Console.WriteLine($"Object {e.SymbolId} -> Position: {e.Position}");
    }

    private static void ObjectAdded(object? sender, Tuio11Object e)
    {
        Console.WriteLine($"New object added -> ID: {e.SymbolId}");
    }

    private static void RemoveCursor(object? sender, Tuio11Cursor cursor)
    {
        Console.WriteLine($"Cursor {cursor.CursorId} removed");
    }

    private static void UpdateCursor(object? sender, Tuio11Cursor cursor)
    {
        Console.WriteLine($"Cursor {cursor.CursorId} -> Position: {cursor.Position}");
    }

    private static void CursorAdded(object? sender, Tuio11Cursor cursor)
    {
        Console.WriteLine($"New cursor added -> ID: {cursor.CursorId}");
    }
}

