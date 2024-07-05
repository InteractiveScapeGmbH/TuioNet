# TuioNet
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-3-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

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

### Tuio Session
The Tuio session class is the entry point for the communication between the client application and a TUIO sender. It takes the TUIO protocol version, the connection type and the IP-Address of the TUIO sender.

Based on the protocol version the `TuioSession` creates a `Processor` which is responsible for parsing the incoming TUIO messages. By default, the `Processor` classes add `MessageListeners` for the currently supported Tuio profiles (see above for a list of the supported profiles). But it is also possible to register for your own custom message profile by calling `AddMessageListener` on the `TuioSession`. Here you are not limited to the Tuio profiles. You can register to every OSC message format your Tuio sender supports.

```csharp
using(var session = new TuioSession(TuioVersion.Tuio11, TuioConnectionType.Websocket, "10.0.0.31"))
{
    session.AddMessageListener(new MessageListener("/my/profile/example", OnMessage);
    
    private void OnMessage(object? sender, OSCMessage message)
    {
        // Do something with the message
    }        
}
```

### Tuio Processors
Tuio processors are responsible for parsing TUIO messages. There are two types of processors, one for TUIO 1.1 and one for TUIO 2.0. They get a client object and can listen to specific messages by register callback methods for them. In the current implementation the TUIO processors listen to the following message profiles.

Tuio11Processor.cs:
```csharp
public Tuio11Processor(TuioClient client)
{
    client.AddMessageListeners(new List<MessageListener>()
    {
        new MessageListener("/tuio/2Dobj", On2Dobj),
        new MessageListener("/tuio/2Dcur", On2Dcur),
        new MessageListener("/tuio/2Dblb", On2Dblb)
    });
    
    TuioTime.Init();
    _currentTime = TuioTime.GetCurrentTime();
}
```

Tuio20Processor.cs
```csharp
public Tuio20Processor(TuioClient client)
{
    client.AddMessageListeners(new List<MessageListener>()
    {
        new MessageListener("/tuio2/frm", OnFrm),
        new MessageListener("/tuio2/alv", OnAlv),
        new MessageListener("/tuio2/tok", OnOther),
        new MessageListener("/tuio2/ptr", OnOther),
        new MessageListener("/tuio2/bnd", OnOther),
        new MessageListener("/tuio2/sym", OnOther),
    });
    
    TuioTime.Init();
    _currentTime = TuioTime.GetCurrentTime();
}
```

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

## Events
In general there are four different kinds of events for TUIO 1.1 and 2.0: `Add`, `Update`, `Remove` and `Refresh`. The `Refresh` event gets triggered at the end of the current frame after all tuio messages were processed and it provides the current `TuioTime`. There are different `Add`, `Update` and `Remove` events for the different kinds of TUIO messages (cursor, object, blob, token...). All possible events are shown below.

### TUIO 1.1
#### OnRefreshed
The Refreshed event is triggered when a TUIO frame is completely processed. This event is useful to handle all updates contained in one TUIO frame together.

```csharp
_processor.OnRefreshed += OnRefreshed;

private void OnRefreshed(object sender, TuioTime time)
{
    // update stuff after the whole tuio frame got processed
}
```

#### OnCursorAdded
This event gets triggered when a new cursor was added this frame.
```csharp
_processor.OnCursorAdded += OnCursorAdded;

private void OnCursorAdded(object sender, Tuio11Cursor cursor)
{
    Console.WriteLine($"New cursor added -> ID: {cursor.CursorId}");
}
```

#### OnCursorUpdated
This event gets triggered when an already known cursor gets updated, for example changes its position.
```csharp
_processor.OnCursorUpdated += OnCursorUpdated;

private void OnCursorUpdated(object sender, Tuio11Cursor cursor)
{
    Console.WriteLine($"Cursor {cursor.CursorId} -> Position: {cursor.Position}");
}
```

#### OnCursorRemoved
This event gets triggered when a cursor was removed this frame.
```csharp
_processor.OnCursorRemoved += OnCursorRemoved;

private void OnCursorAdded(object sender, Tuio11Cursor cursor)
{
    Console.WriteLine($"Cursor {cursor.CursorId} removed");
}
```

#### OnObjectAdded
This event gets triggered when a new TUIO 1.1 object was added this frame.
```csharp
_processor.OnObjectAdded += OnObjectAdded;

private void OnObjectAdded(object sender, Tuio11Object tuioObject)
{
    Console.WriteLine($"New object added -> ID: {tuioObject.SymbolId}");
}
```

#### OnObjectUpdated
This event gets triggered when an already known TUIO 1.1 object gets updated, for example changes its position or rotation.
```csharp
_processor.OnObjectUpdated += OnObjectUpdated;

private void OnObjectUpdated(object sender, Tuio11Object tuioObject)
{
    Console.WriteLine($"Object {tuioObject.SymbolId} -> Position: {tuioObject.Position}");
}
```

#### OnObjectRemoved
This event gets triggered when a TUIO 1.1  object was removed this frame.
```csharp
_processor.OnObjectRemoved += OnObjectRemoved;

private void OnObjectRemoved(object sender, Tuio11Object tuioObject)
{
    Console.WriteLine($"Object {tuioObject.SymbolId} removed");
}
```

#### OnBlobAdded
This event gets triggered when a new TUIO 1.1 blob was added this frame.
```csharp
_processor.OnBlobAdded += OnBlobAdded;

private void OnBlobAdded(object sender, Tuio11Blob blob)
{
    Console.WriteLine($"New Blob added -> ID: {blob.BlobId}");
}
```

#### OnBlobUpdated
This event gets triggered when an already known TUIO 1.1 blob gets updated, for example changes its position or rotation.
```csharp
_processor.OnBlobUpdated += OnBlobUpdated;

private void OnBlobUpdated(object sender, Tuio11Blob blob)
{
    Console.WriteLine($"Blob {blob.BlobId} -> Position: {blob.Position}");
}
```

#### OnBlobRemoved
This event gets triggered when a TUIO 1.1 blob was removed this frame.
```csharp
_processor.OnBlobRemoved += OnBlobRemoved;

private void OnBlobRemoved(object sender, Tuio11Blob blob)
{
    Console.WriteLine($"Blob {blob.BlobId} removed");
}
```

### TUIO 2.0
#### OnRefreshed
The Refreshed event is triggered when a TUIO frame is completely processed. This event is useful to handle all updates contained in one TUIO frame together.

```csharp
_processor.OnRefreshed += OnRefreshed;

private void OnRefreshed(object sender, TuioTime time)
{
    // update stuff after the whole tuio frame got processed
}

```
#### OnObjectAdded
This event gets triggered when a new TUIO 2.0 object was added this frame. This could be a pointer, token, bounds or symbol.
```csharp
_processor.OnObjectAdded += OnObjectAdded;

private void OnObjectAdded(object sender, Tuio20Object tuioObject)
{
    // if you expect the object to be a pointer you can check it and act accordingly
    if(tuioObject.ContainsTuioPointer())
    {
        Console.WriteLine($"New Pointer added -> ID: {tuioObject.Pointer.ComponentId}");
    }
            
}
```

#### OnObjectUpdated
This event gets triggered when an already known TUIO 2.0 object gets updated, for example changes its position.
```csharp
_processor.OnObjectUpdated += OnObjectUpdated;

private void OnObjectUpdated(object sender, Tuio20Object tuioObject)
{
    // if you expect the object to be a token you can check it and act accordingly
    if(tuioObject.ContainsTuioToken())
    {
        Console.WriteLine($"Token {tuioObject.Token.ComponentId} -> Position: {tuioObject.Token.Position}");
    }
}
```

#### OnObjectRemoved
This event gets triggered when a TUIO 2.0 object was removed this frame.
```csharp
_processor.OnObjectRemoved += OnObjectRemoved;

private void OnObjectRemoved(object sender, Tuio20Object tuioObject)
{
    // if you expect the object to be a symbol you can check it and act accordingly
    if(tuioObject.ContainsTuioSymbol())
    {
        Console.WriteLine($"Symbol {tuioObject.Symbol.ComponentId} removed");
    }
}
```

## Demo Console Application
A simple console application which demonstrates a simple setup for a TUIO 1.1 connection via UDP. 
- First create a `TuioSession` (the default port is 3333)
- Get the `ITuioDispatcher` from the session object and cast it to the appropriate type based on the TUIO version (`Tuio11Dispatcher` or `Tuio20Dispatcher`).
- Register methods for the desired events on the dispatcher object.
- The connecting and disconnecting is handled by the session object. As soon as the session gets disposed it disconnects from the sender.
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
        using (var tuioSession = new TuioSession(TuioVersion.Tuio11, TuioConnectionType.UDP))
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
```

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://www.interactive-scape.com/"><img src="https://avatars.githubusercontent.com/u/51314413?v=4?s=100" width="100px;" alt="Erich Querner"/><br /><sub><b>Erich Querner</b></sub></a><br /><a href="https://github.com/InteractiveScapeGmbH/TuioNet/commits?author=eqbic" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/gilescoope"><img src="https://avatars.githubusercontent.com/u/5291605?v=4?s=100" width="100px;" alt="Giles Coope"/><br /><sub><b>Giles Coope</b></sub></a><br /><a href="https://github.com/InteractiveScapeGmbH/TuioNet/commits?author=gilescoope" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="http://modin.yuri.at/"><img src="https://avatars.githubusercontent.com/u/115223?v=4?s=100" width="100px;" alt="Martin Kaltenbrunner"/><br /><sub><b>Martin Kaltenbrunner</b></sub></a><br /><a href="https://github.com/InteractiveScapeGmbH/TuioNet/commits?author=mkalten" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
