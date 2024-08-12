using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        using (var tuioSession = new TuioSession(TuioVersion.Tuio20, TuioConnectionType.UDP))
        {
            var dispatcher = (Tuio20Dispatcher)tuioSession.TuioDispatcher;
            dispatcher.OnObjectAdd += Tuio20Add;
            dispatcher.OnObjectUpdate += Tuio20Update;
            dispatcher.OnObjectRemove += Tuio20Remove;
            // dispatcher.OnCursorAdd += CursorAdded;
            // dispatcher.OnCursorUpdate += UpdateCursor;
            // dispatcher.OnCursorRemove += RemoveCursor;
            //
            // dispatcher.OnObjectAdd += ObjectAdded;
            // dispatcher.OnObjectUpdate += UpdateObject;
            // dispatcher.OnObjectRemove += RemoveObject;
            Console.WriteLine("Connect...");
            while (true)
            {
                if (!Console.KeyAvailable) continue;
                var pressedKey = Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.Q) break;
            }
            Console.WriteLine("Disconnect...");
            // dispatcher.OnCursorAdd -= CursorAdded;
            // dispatcher.OnCursorUpdate -= UpdateCursor;
            // dispatcher.OnCursorRemove -= RemoveCursor;
            //
            // dispatcher.OnObjectAdd -= ObjectAdded;
            // dispatcher.OnObjectUpdate -= UpdateObject;
            // dispatcher.OnObjectRemove -= RemoveObject;
            dispatcher.OnObjectAdd -= Tuio20Add;
            dispatcher.OnObjectUpdate -= Tuio20Update;
            dispatcher.OnObjectRemove -= Tuio20Remove;
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

