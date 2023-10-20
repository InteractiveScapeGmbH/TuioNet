using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioClient = new Tuio11Client(TuioConnectionType.UDP);
        tuioClient.AddTuioListener(new Listener11());
        Console.WriteLine("Connect...");
        tuioClient.Connect();
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            var pressedKey = Console.ReadKey().Key;
            if (pressedKey == ConsoleKey.Q) break;
        }
        Console.WriteLine("Disconnect...");
        tuioClient.Disconnect();
    }
}

public class Listener11 : ITuio11Listener
{
    public void TuioAdd(Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"Cursor {tuio20Object.Pointer.SessionId} -> Position: {tuio20Object.Pointer.Position}");
        }
    }

    public void TuioUpdate(Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"Cursor {tuio20Object.Pointer.SessionId} -> Position: {tuio20Object.Pointer.Position}, Velocity: {tuio20Object.Pointer.Velocity}, Speed: {tuio20Object.Pointer.Speed}, Time: {tuio20Object.Pointer.CurrentTime}");
        }
    }

    public void TuioRemove(Tuio20Object tuio20Object)
    {
        if (tuio20Object.ContainsTuioPointer())
        {
            Console.WriteLine($"Cursor {tuio20Object.Pointer.SessionId} -> Position: {tuio20Object.Pointer.Position}");
        }
    }

    public void TuioRefresh(TuioTime tuioTime)
    {
        // throw new NotImplementedException();
    }

    public void AddTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} added.");
    }

    public void UpdateTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Cursor {tuio11Object.SymbolId} -> Position: {tuio11Object.Position}, Velocity: {tuio11Object.Velocity}, Speed: {tuio11Object.Speed}, Time: {tuio11Object.CurrentTime}");
    }

    public void RemoveTuioObject(Tuio11Object tuio11Object)
    {
        Console.WriteLine($"Object {tuio11Object.SymbolId} removed.");

    }

    public void AddTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.SessionId} added.");

    }

    public void UpdateTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.SessionId} -> Position: {tuio11Cursor.Position}, Velocity: {tuio11Cursor.Velocity}, Speed: {tuio11Cursor.Speed}, Time: {tuio11Cursor.CurrentTime}");
    }

    public void RemoveTuioCursor(Tuio11Cursor tuio11Cursor)
    {
        Console.WriteLine($"Cursor {tuio11Cursor.SessionId} removed.");
    }

    public void AddTuioBlob(Tuio11Blob tuio11Blob)
    {
    }

    public void UpdateTuioBlob(Tuio11Blob tuio11Blob)
    {
    }

    public void RemoveTuioBlob(Tuio11Blob tuio11Blob)
    {
    }

    public void Refresh(TuioTime tuioTime)
    {
    }
}

