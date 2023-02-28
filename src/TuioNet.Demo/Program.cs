using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioReceiver = new UdpTuioReceiver(3333, false);
        var tuioClient = new Tuio11Client(tuioReceiver);
        tuioClient.AddTuioListener(new TuioListener());
        Console.WriteLine("Connect...");
        tuioClient.Connect();
        while (true)
        {
            tuioReceiver.ProcessMessages();
            if (!Console.KeyAvailable) continue;
            var pressedKey = Console.ReadKey().Key;
            if (pressedKey == ConsoleKey.Q) break;
        }
        Console.WriteLine("Disconnect...");
        tuioClient.Disconnect();
    }
}

