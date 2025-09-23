using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Client.Tuio11;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.Tests.Tuio11;

[TestFixture]
public class Test_Tuio11_Send
{
    private string sourceName;
    private Tuio11Manager manager;

    [SetUp]
    public void Init()
    {
        sourceName = "test-source";
        TuioTime.Init();
        manager = new Tuio11Manager(sourceName);
    }
    
    [Test]
    public void Test_Cursor_Send()
    {
        Tuio11Cursor cursor = new Tuio11Cursor(TuioTime.GetSystemTime(), 1234, 6543, Vector2.One, Vector2.Zero, 0f);
        manager.AddCursor(cursor);
        var bundle = manager.FrameBundle;
        var byteBundle = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteBundle);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetCursorMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        var processor = new Tuio11Processor();
        processor.On2Dcur(this, targetSourceMessage);
        processor.On2Dcur(this, targetAliveMessage);
        processor.On2Dcur(this, targetCursorMessage);
        processor.On2Dcur(this, targetFseqMessage);
        
        Assert.AreEqual(1, processor.GetTuioCursors().Count);
        var receivedCursor = processor.GetTuioCursors()[0];
        
        Assert.AreEqual(sourceName, processor.Source);
        Assert.AreEqual(1234, receivedCursor.SessionId);
    }   
    
    [Test]
    public void Test_Object_Send()
    {
        Tuio11Object tuioObject = new Tuio11Object(TuioTime.GetSystemTime(), 1234, 4321, Vector2.Zero, 0f, Vector2.One,
            0f, 0f, 0f);

        manager.AddObject(tuioObject);
        var bundle = manager.FrameBundle;
        var byteBundle = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteBundle);
        var targetSourceMessage = (OSCMessage)packet.Values[3];
        var targetAliveMessage = (OSCMessage)packet.Values[4];
        var targetCursorMessage = (OSCMessage)packet.Values[5];
        var targetFseqMessage = (OSCMessage)packet.Values[6];

        var processor = new Tuio11Processor();
        processor.On2Dobj(this, targetSourceMessage);
        processor.On2Dobj(this, targetAliveMessage);
        processor.On2Dobj(this, targetCursorMessage);
        processor.On2Dobj(this, targetFseqMessage);
        
        Assert.AreEqual(1, processor.GetTuioObjects().Count);
        Assert.AreEqual(sourceName, processor.Source);

        var receivedObject = processor.GetTuioObjects()[0];
        Assert.AreEqual(4321, receivedObject.SymbolId);
    }
    
    [Test]
    public void Test_Blob_Send()
    {
        Tuio11Blob blob = new Tuio11Blob(TuioTime.GetSystemTime(), 1234, 4321, Vector2.Zero, 0f, Vector2.One, 1f,
            Vector2.Zero, 0f, 0f, 0f);

        manager.AddBlob(blob);
        var bundle = manager.FrameBundle;
        var byteBundle = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteBundle);
        var targetSourceMessage = (OSCMessage)packet.Values[6];
        var targetAliveMessage = (OSCMessage)packet.Values[7];
        var targetCursorMessage = (OSCMessage)packet.Values[8];
        var targetFseqMessage = (OSCMessage)packet.Values[9];

        var processor = new Tuio11Processor();
        processor.On2Dblb(this, targetSourceMessage);
        processor.On2Dblb(this, targetAliveMessage);
        processor.On2Dblb(this, targetCursorMessage);
        processor.On2Dblb(this, targetFseqMessage);
        
        Assert.AreEqual(1, processor.GetTuioBlobs().Count);
        Assert.AreEqual(sourceName, processor.Source);

        var receivedBlob = processor.GetTuioBlobs()[0];
        Assert.AreEqual(1234, receivedBlob.SessionId);
    }   
}