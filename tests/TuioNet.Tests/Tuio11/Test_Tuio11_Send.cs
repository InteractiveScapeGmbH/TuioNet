using System.Numerics;
using NUnit.Framework;
using TuioNet.Client.Tuio11;
using TuioNet.Common;
using TuioNet.OSC;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.Tests.Tuio11;

[TestFixture]
public class TestTuio11Send
{
    private string _sourceName;
    private Tuio11Manager _manager;

    [SetUp]
    public void Init()
    {
        _sourceName = "test-source";
        TuioTime.Init();
        _manager = new Tuio11Manager(_sourceName);
    }
    
    [Test]
    public void Test_Cursor_Send()
    {
        var cursor = new Tuio11Cursor(TuioTime.GetSystemTime(), 1234, 6543, Vector2.One, Vector2.Zero, 0f);
        _manager.AddCursor(cursor);
        var bundle = _manager.FrameBundles[0];
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
        
        Assert.That(processor.GetTuioCursors().Count, Is.EqualTo(1));
        var receivedCursor = processor.GetTuioCursors()[0];
        
        Assert.That(processor.Source, Is.EqualTo(_sourceName));
        Assert.That(receivedCursor.SessionId, Is.EqualTo(1234));
    }   
    
    [Test]
    public void Test_Object_Send()
    {
        var tuioObject = new Tuio11Object(TuioTime.GetSystemTime(), 1235, 4321, Vector2.Zero, 0f, Vector2.One,
            0f, 0f, 0f);

        _manager.AddObject(tuioObject);
        var bundle = _manager.FrameBundles[1];
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
        Assert.Multiple(() =>
        {
            Assert.That(processor.GetTuioObjects(), Has.Count.EqualTo(1));
            Assert.That(processor.Source, Is.EqualTo(_sourceName));
        });
        var receivedObject = processor.GetTuioObjects()[0];
        Assert.That(receivedObject.SymbolId, Is.EqualTo(4321));
    }

    [Test]
    public void Test_Blob_Send()
    {
        var blob = new Tuio11Blob(TuioTime.GetSystemTime(), 1234, 4321, Vector2.Zero, 0f, Vector2.One, 1f,
            Vector2.Zero, 0f, 0f, 0f);

        _manager.AddBlob(blob);
        var bundle = _manager.FrameBundles[2];
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
        Assert.Multiple(() =>
        {
            Assert.That(processor.GetTuioBlobs(), Has.Count.EqualTo(1));
            Assert.That(processor.Source, Is.EqualTo(_sourceName));
        });
        var receivedBlob = processor.GetTuioBlobs()[0];
        Assert.That(receivedBlob.SessionId, Is.EqualTo(1234));
    }
}