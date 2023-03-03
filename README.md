# TuioNet
[![All Contributors](https://img.shields.io/github/all-contributors/InteractiveScapeGmbH/TuioNet?color=ee8449&style=flat-square)](#contributors)

TuioNet is a .Net implementation of the TUIO specification by Martin Kaltenbrunner. It supports [TUIO 1.1](http://tuio.org/?specification) and [TUIO 2.0](http://www.tuio.org/?tuio20) for the following message profiles:

**TUIO 1.1**
```
/tuio/2Dobj
/tuio/2Dcur
/tuio/2Dblb
```

**TUIO 2.0**
```
/tuio2/frm
/tuio2/alv
/tuio2/tok 
/tuio2/ptr 
/tuio2/bnd 
/tuio2/sym 
```

## Documentation
A brief overview over the most important classes and how to use them.

### Tuio Clients
The Tuio client classes are the entry point for the communication between the client application and a TUIO sender. They hold a reference to a TuioReceiver (which can be UDP or Websocket receiver).
They can listen to specific messages and register callback methods for them. In the current implementation the TUIO clients listen to the following message profiles.

Tuio11Client.cs:
```csharp
public Tuio11Client(TuioConnectionType connectionType, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)  
{  
    _tuioReceiver = TuioReceiver.FromConnectionTye(connectionType, address, port, isAutoProcess;  
    _tuioReceiver.AddMessageListener("/tuio/2Dobj", On2Dobj);  
    _tuioReceiver.AddMessageListener("/tuio/2Dcur", On2Dcur);  
    _tuioReceiver.AddMessageListener("/tuio/2Dblb", On2Dblb);  
}
```

Tuio20Client.cs
```csharp
public Tuio20Client(TuioConnectionType connectionType, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)  
{  
    _tuioReceiver = TuioReceiver.FromConnectionTye(connectionType, address, port, isAutoProcess;  
    _tuioReceiver.AddMessageListener("/tuio2/frm", OnFrm);  
    _tuioReceiver.AddMessageListener("/tuio2/alv", OnAlv);  
    _tuioReceiver.AddMessageListener("/tuio2/tok", OnOther);  
    _tuioReceiver.AddMessageListener("/tuio2/ptr", OnOther);  
    _tuioReceiver.AddMessageListener("/tuio2/bnd", OnOther);  
    _tuioReceiver.AddMessageListener("/tuio2/sym", OnOther);  
}
```
#### Add/Remove TuioListeners
If your application should get informed when TUIO objects get added, updated or removed you need to implement the ```ITuio11Listener``` (for TUIO 1.1) or the ```ITuio20Listener``` (for TUIO 2.0) interface
which provides methods for add, update and remove callbacks for the different kinds of TUIO types (pointer, cursor, object, token...). As soon as you have a class implemented a TuioListener interface you 
can add the listener to your client via the ```AddTuioListener()``` method. An example of how this looks like is given in the demo console application.

### TuioReceiver.cs
An abstract base class for different connection types for receiving messages from a TUIO source. Currently there
are two types of receivers implemented: ```UdpTuioReceiver.cs``` and ```WebsocketTuioReceiver.cs```.

#### Add/Remove Message Listeners
The TUIO Specification ([1.1](http://tuio.org/?specification) and [2.0](http://www.tuio.org/?tuio20)) defines different kinds of
TUIO message profiles:</br>

TUIO 1.1 Examples:
```
/tuio/2Dobj set s i x y a X Y A m r
/tuio/2Dcur set s x y X Y m
/tuio/2Dblb set s x y a w h f X Y A m r
```

TUIO 2.0 Examples:
```
/tuio2/tok s_id tu_id c_id x_pos y_pos angle [x_vel y_vel m_acc r_vel r_acc]
/tuio2/ptr s_id tu_id c_id x_pos y_pos angle shear radius press [x_vel y_vel p_vel m_acc p_acc]
/tuio2/bnd s_id x_pos y_pos angle width height area [x_vel y_vel a_vel m_acc r_acc]
/tuio2/sym s_id tu_id c_id t_des data
```

#### Process Messages
The TuioReceiver class is responsible for processing the TUIO messages which it receives from the sender. 
The ```OnBuffer``` method is called everytime a new message gets received and it puts the new message in a message
queue. The ```ProcessMessages``` method is looking for new messages in the message queue and calls the associated
callback methods the message listeners registered to them. If the ```isAutoProcess``` flag is set to ```true``` the ```ProcessMessages```
method is called automatically. Otherwise it needs to be called manually.

## Demo Console Application
A simple console application which demonstrates a simple setup for a TUIO 1.1 connection via UDP. 
- First create a Tuio11Client (the default port is 3333, you can set it in the constructor of the client class)
- Add a new Tuio11Listener to the client.
- Establish a connection to the TUIO sender.
- The application runs and prints to the console when TUIO objects/cursors appear, move or get removed.
- By pressing the ```Q``` button you can stop the application.

Program.cs
```csharp
using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

class Program
{
    private static void Main(string[] args)
    {
        var tuioClient = new Tuio11Client(TuioConnectionType.UDP);
        tuioClient.AddTuioListener(new TuioListener());
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
```

An example implementation of the ITuio11Listener interface which prints to the console.

TuioListener.cs
```csharp
using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Demo;

public class TuioListener : ITuio11Listener
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
```

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
