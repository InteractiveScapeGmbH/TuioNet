using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Client.Tuio11;
using TuioNet.Common;
using TuioNet.Tuio11;

namespace TuioNet.Tests.Tuio11;

[TestFixture]
public class Test_Tuio11_Receive
{
    private uint sId;
    private uint symbolId;
    private Vector2 position;
    private float angle;
    private Vector2 velocity;
    private float rotationSpeed;
    private float acceleration;
    private float rotationAcceleration;
    private string sourceName;
    private uint frameId;
    private uint cursorId;
    private uint blobId;
    private Vector2 size;
    private float area;
    
    private Tuio11Processor processor;
    
    [SetUp]
    public void Init()
    {
        sId = uint.MaxValue;
        symbolId = uint.MaxValue;
        position = new Vector2(2.345f, 54.123f);
        angle = float.MaxValue;
        velocity = new Vector2(1.23f, 5.1123f);
        rotationSpeed = float.MaxValue;
        acceleration = float.MaxValue;
        rotationAcceleration = float.MaxValue;
        sourceName = "test-source";
        processor = new Tuio11Processor();
        frameId = 5;
        cursorId = uint.MaxValue;
        blobId = uint.MaxValue;
        size = new Vector2(12.34f, 432.12f);
        area = float.MaxValue;
        TuioTime.Init();
    }

    private OSCMessage SetSourceMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("source");
        message.Append(sourceName);
        return message;
    }

    private OSCMessage SetAliveMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("alive");
        message.Append((int)sId);
        return message;
    }

    private OSCMessage SetFseqMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("fseq");
        message.Append((int)frameId);
        return message;
    }
    
    [Test]
    public void Test_Tuio11_Cursor_OSC_Receive()
    {
        var tuioAddress = "/tuio/2Dcur";
        var tuioObject = new Tuio11Cursor(TuioTime.GetSystemTime(), sId, cursorId, position, velocity, acceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioObject.OscMessage;
        bundle.Append(SetSourceMessage(tuioAddress));
        bundle.Append(SetAliveMessage(tuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(tuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetCursorMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        processor.On2Dcur(this, targetSourceMessage);
        processor.On2Dcur(this, targetAliveMessage);
        processor.On2Dcur(this, targetCursorMessage);
        processor.On2Dcur(this, targetFseqMessage);

        Assert.AreEqual(1, processor.GetTuioCursors().Count);

        var targetCursor = processor.GetTuioCursors()[0];
        Assert.AreEqual(sId, targetCursor.SessionId);
        Assert.AreEqual(position, targetCursor.Position);
        Assert.AreEqual(velocity, targetCursor.Velocity);
        Assert.AreEqual(acceleration, targetCursor.Acceleration);
    }
    

    [Test]
    public void Test_Tuio11_Object_OSC_Receive()
    {
        var tuioAddress = "/tuio/2Dobj";
        var tuioObject = new Tuio11Object(TuioTime.GetSystemTime(), sId, symbolId, position, angle, velocity,
            rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioObject.OscMessage;
        bundle.Append(SetSourceMessage(tuioAddress));
        bundle.Append(SetAliveMessage(tuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(tuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetObjectMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        processor.On2Dobj(this, targetSourceMessage);
        processor.On2Dobj(this, targetAliveMessage);
        processor.On2Dobj(this, targetObjectMessage);
        processor.On2Dobj(this, targetFseqMessage);

        Assert.AreEqual(1, processor.GetTuioObjects().Count);

        var targetObject = processor.GetTuioObjects()[0];
        Assert.AreEqual(sId, targetObject.SessionId);
        Assert.AreEqual(symbolId, targetObject.SymbolId);
        Assert.AreEqual(position, targetObject.Position);
        Assert.AreEqual(angle, targetObject.Angle);
        Assert.AreEqual(velocity, targetObject.Velocity);
        Assert.AreEqual(rotationSpeed, targetObject.RotationSpeed);
        Assert.AreEqual(acceleration, targetObject.Acceleration);
        Assert.AreEqual(rotationAcceleration, targetObject.RotationAcceleration);
    }
    
    [Test]
    public void Test_Tuio11_Blob_OSC_Receive()
    {
        var tuioAddress = "/tuio/2Dblb";
        var tuioBlob = new Tuio11Blob(TuioTime.GetSystemTime(), sId, blobId, position, angle, size, area, velocity, rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioBlob.OscMessage;
        bundle.Append(SetSourceMessage(tuioAddress));
        bundle.Append(SetAliveMessage(tuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(tuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetBlobMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        processor.On2Dblb(this, targetSourceMessage);
        processor.On2Dblb(this, targetAliveMessage);
        processor.On2Dblb(this, targetBlobMessage);
        processor.On2Dblb(this, targetFseqMessage);

        Assert.AreEqual(1, processor.GetTuioBlobs().Count);

        var targetBlob = processor.GetTuioBlobs()[0];
        Assert.AreEqual(sId, targetBlob.SessionId);
        Assert.AreEqual(position, targetBlob.Position);
        Assert.AreEqual(angle, targetBlob.Angle);
        Assert.AreEqual(size, targetBlob.Size);
        Assert.AreEqual(area, targetBlob.Area);
        Assert.AreEqual(velocity, targetBlob.Velocity);
        Assert.AreEqual(rotationSpeed, targetBlob.RotationSpeed);
        Assert.AreEqual(acceleration, targetBlob.Acceleration);
        Assert.AreEqual(rotationAcceleration, targetBlob.RotationAcceleration);
    }
}