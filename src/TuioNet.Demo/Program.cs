using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        using (var tuioSession = new TuioSession(logger, TuioVersion.Tuio11, TuioConnectionType.UDP))
        {
            var dispatcher = (Tuio11Dispatcher)tuioSession.TuioDispatcher;
            dispatcher.OnCursorAdd += CursorAdded;
            dispatcher.OnCursorUpdate += UpdateCursor;
            dispatcher.OnCursorRemove += RemoveCursor;
            Console.WriteLine("Connect...");
            while (true)
            {
                if (!Console.KeyAvailable) continue;
                var pressedKey = Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.Q) break;
            }
            Console.WriteLine("Disconnect...");
            dispatcher.OnCursorAdd -= CursorAdded;
            dispatcher.OnCursorUpdate -= UpdateCursor;
            dispatcher.OnCursorRemove -= RemoveCursor;
        }
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

