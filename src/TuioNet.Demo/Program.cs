using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioClient = new TuioClient(TuioConnectionType.UDP);
        var processor = new Tuio11Processor(tuioClient);
        processor.OnCursorAdded += CursorAdded;
        processor.OnCursorUpdated += UpdateCursor;
        processor.OnCursorRemoved += RemoveCursor;
        Console.WriteLine("Connect...");
        tuioClient.Connect();
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            var pressedKey = Console.ReadKey().Key;
            if (pressedKey == ConsoleKey.Q) break;
        }
        Console.WriteLine("Disconnect...");
        processor.OnCursorAdded -= CursorAdded;
        processor.OnCursorUpdated -= UpdateCursor;
        processor.OnCursorRemoved -= RemoveCursor;
        tuioClient.Disconnect();
    }

    private static void CursorAdded(Tuio11Cursor cursor)
    {
        Console.WriteLine($"New cursor added -> ID: {cursor.CursorId}");
    }

    private static void UpdateCursor(Tuio11Cursor cursor)
    {
        Console.WriteLine($"Cursor {cursor.CursorId} -> Position: {cursor.Position}");
    }

    private static void RemoveCursor(Tuio11Cursor cursor)
    {
        Console.WriteLine($"Cursor {cursor.CursorId} removed");
    }
}

