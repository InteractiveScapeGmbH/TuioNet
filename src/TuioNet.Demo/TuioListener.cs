using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

public class TuioListener : Tuio11Listener
{
    public void AddTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"New object added -> ID: {tuio11Object.SymbolId}");
    }

    public void UpdateTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} -> Position: {tuio11Object.Position}, Angle: {tuio11Object.Angle}");
    }

    public void RemoveTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} removed");
    }

    public void AddTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"New cursor added -> ID: {tuio11Cursor.CursorId}");
    }

    public void UpdateTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.CursorId} -> Position: {tuio11Cursor.Position}");
    }

    public void RemoveTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.CursorId} removed");
    }

    public void AddTuioBlob(Tuio11Blob tuio11Blob)
    {
        Console.WriteLine($"New blob added -> ID: {tuio11Blob.BlobId}");
    }

    public void UpdateTuioBlob(Tuio11Blob tuio11Blob)
    {
        Console.WriteLine($"Blob {tuio11Blob.BlobId} -> Position: {tuio11Blob.Position}, Angle: {tuio11Blob.Angle}, Area: {tuio11Blob.Area}");
    }

    public void RemoveTuioBlob(Tuio11Blob tuio11Blob)
    {
        Console.WriteLine($"Blob {tuio11Blob.BlobId} removed.");
    }

    public void Refresh(TuioTime tuioTime)
    {
    }
}