using TuioNet.Common;
using TuioNet.Tuio11;
using TuioNet.Tuio20;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioClient = new Tuio20Client(TuioConnectionType.UDP);
        tuioClient.AddTuioListener(new Listener20());
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

public class Listener20 : ITuio20Listener
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
}

