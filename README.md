# TuioNet
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-3-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

TuioNet is a .Net implementation of the TUIO specification by Martin Kaltenbrunner. It provides packages to implement Tuio Sender and Tuio Receiver. (see Demo Projects)

It supports [TUIO 1.1](http://tuio.org/?specification) and [TUIO 2.0](http://www.tuio.org/?tuio20) for the following message profiles:

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

## Demo Console Applications
There are two examples how one can implement a Tuio Sender and Tuio Receiver.

### TuioNet.ServerDemo
The Server simulates a Touch and an Object moving in circles.

```shell
❯ ./TuioNet.ServerDemo --help
TuioNet.ServerDemo 1.0.0+f92ea0876914d47908d2f57c8c3265504e935f1f
Copyright (C) 2026 TuioNet.ServerDemo

  -i, --ip             (Default: 127.0.0.1) Set the ip address of the tuio sender.

  -p, --port           (Default: 3333) Set the port.

  -l, --logLevel       (Default: Information) Set the minimum log level [Trace, Debug, Information, Warning, Error, Critical, None].

  -v, --tuioVersion    (Default: Tuio11) Set the tuio version [Tuio11, Tuio20].

  -c, --connection     (Default: UDP) Set the connection type [UDP, Websocket].

  -s, --sourceName     (Default: TuioServerDemo) Set the source name for the Tuio Sender.

  -r, --resolution     (Default: 1280,720) Set the screen resolution as [x,y]. Only valid for Tuio 2.

  --help               Display this help screen.

  --version            Display version information.
```

Example (for Tuio 2.0 over Websocket)
```shell
❯ ./TuioNet.ServerDemo -l Debug -c Websocket -v Tuio20
dbug: TuioNet.ServerDemo.Program[0]
      /tuio2/frm 1 1/16/2026 4:12:35 PM 83886800 TuioServerDemo 
      /tuio2/ptr 0 0 0 0.89968 0.5159958 0 0 0 0 0 0 0 0 0 
      /tuio2/tok 1 0 5 0.8980017 0.5399335 0.10000035 0 0 0 0 0 
      /tuio2/alv 0 1 
      
dbug: TuioNet.ServerDemo.Program[0]
      /tuio2/frm 2 1/16/2026 4:12:35 PM 83886800 TuioServerDemo 
      /tuio2/ptr 0 0 0 0.8987207 0.531966 0 0 0 0 0 0 0 0 0 
      /tuio2/tok 1 0 5 0.8920266 0.5794679 0.2000004 0 0 0 0 0 
      /tuio2/alv 0 1 
      
dbug: TuioNet.ServerDemo.Program[0]
      /tuio2/frm 3 1/16/2026 4:12:35 PM 83886800 TuioServerDemo 
      /tuio2/ptr 0 0 0 0.89712316 0.5478873 0 0 0 0 0 0 0 0 0 
      /tuio2/tok 1 0 5 0.88213277 0.618214 0.30001557 0 0 0 0 0 
      /tuio2/alv 0 1 
```

### TuioNet.ClientDemo

First run the TuioNet.ServerDemo. Then start the TuioNet.ClientDemo with the same parameters in order to receive Tuio messages.

```shell
❯ ./TuioNet.ClientDemo --help
TuioNet.ClientDemo 1.0.0+f92ea0876914d47908d2f57c8c3265504e935f1f
Copyright (C) 2026 TuioNet.ClientDemo

  -i, --ip             (Default: 127.0.0.1) Set the ip address of the tuio sender.

  -p, --port           (Default: 3333) Set the port.

  -l, --logLevel       (Default: Information) Set the minimum log level [Trace, Debug, Information, Warning, Error, Critical, None].

  -v, --tuioVersion    (Default: Tuio11) Set the tuio version [Tuio11, Tuio20].

  -c, --connection     (Default: UDP) Set the connection type [UDP, Websocket].

  --help               Display this help screen.

  --version            Display version information.
```

Example (for Tuio 2.0 over Websocket)
```shell
❯ ./TuioNet.ClientDemo -l Debug -c Websocket -v Tuio20
info: TuioNet.ClientDemo.Program[0]
      [WebsocketClient] Try to connect to: ws://127.0.0.1:3333/
info: TuioNet.ClientDemo.Program[0]
      [TuioSession] Session initialized!
Connect...
info: TuioNet.ClientDemo.Program[0]
      [WebsocketClient] Connected to ws://127.0.0.1:3333/.
[Tuio 2.0] Pointer added -> SessionId: 0
[Tuio 2.0] Token added -> SessionId: 1, ComponentId: 5
[Tuio 2.0] Pointer 0 -> Position: <0.40796626, 0.11073172>
[Tuio 2.0] Token 5 -> Position: <0.5814245, 0.1083751>, Angle: 11.200568
[Tuio 2.0] Pointer 0 -> Position: <0.42360643, 0.10736272>
[Tuio 2.0] Token 5 -> Position: <0.6201151, 0.118460536>, Angle: 11.300569
[Tuio 2.0] Pointer 0 -> Position: <0.43936884, 0.10462189>
[Tuio 2.0] Token 5 -> Position: <0.6576055, 0.1323582>, Angle: 11.400569
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
