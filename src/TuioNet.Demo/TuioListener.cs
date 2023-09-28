using TuioNet.Tuio11;

namespace TuioNet.Demo;

public class TuioListener : ITuio11CursorListener, ITuio11ObjectListener
{
    public void AddObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"New object added -> ID: {tuio11Object.SymbolId}");
    }

    public void UpdateObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} -> Position: {tuio11Object.Position}, Angle: {tuio11Object.Angle}");
    }

    public void RemoveObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} removed");
    }

    public void AddCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"New cursor added -> ID: {tuio11Cursor.CursorId}");
    }

    public void UpdateCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.CursorId} -> Position: {tuio11Cursor.Position}");
    }

    public void RemoveCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.CursorId} removed");
    }
}