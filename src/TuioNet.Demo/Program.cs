using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioClient = new TuioClient(TuioConnectionType.UDP);
        var processor = new Tuio11Processor(tuioClient);
        processor.OnObjectAdded += AddObject;
        processor.OnObjectUpdated += UpdateObject;
        processor.OnObjectRemoved += RemoveObject;
        Console.WriteLine("Connect...");
        tuioClient.Connect();
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            var pressedKey = Console.ReadKey().Key;
            if (pressedKey == ConsoleKey.Q) break;
        }
        Console.WriteLine("Disconnect...");
        processor.OnObjectAdded -= AddObject;
        processor.OnObjectUpdated -= UpdateObject;
        processor.OnObjectRemoved -= RemoveObject;
        tuioClient.Disconnect();
    }

    private static void AddObject(Tuio11Object tuioObject)
    {
        Console.WriteLine($"New cursor added -> ID: {tuioObject.SymbolId}, Position: {tuioObject.Position}");
    }

    private static void UpdateObject(Tuio11Object tuioObject)
    {
        Console.WriteLine($"Cursor {tuioObject.SymbolId} -> Position: {tuioObject.Position}");
    }

    private static void RemoveObject(Tuio11Object tuioObject)
    {
        Console.WriteLine($"Cursor {tuioObject.SymbolId} removed");
    }
}

