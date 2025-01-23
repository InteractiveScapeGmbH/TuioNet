using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        // Setup TUIO version and connection type
        var tuioVersion = TuioVersion.Tuio11;
        var connectionType = TuioConnectionType.Websocket;
        using var tuioSession = new TuioSession(logger, tuioVersion, connectionType);
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

