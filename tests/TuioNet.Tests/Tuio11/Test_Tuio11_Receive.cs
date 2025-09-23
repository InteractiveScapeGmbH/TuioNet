using System.Numerics;
using NUnit.Framework;
using TuioNet.Client.Tuio11;
using TuioNet.Common;
using TuioNet.OSC;
using TuioNet.Tuio11;

namespace TuioNet.Tests.Tuio11;

[TestFixture]
public class TestTuio11Receive
{
    private uint _sId;
    private uint _symbolId;
    private Vector2 _position;
    private float _angle;
    private Vector2 _velocity;
    private float _rotationSpeed;
    private float _acceleration;
    private float _rotationAcceleration;
    private string _sourceName;
    private uint _frameId;
    private uint _cursorId;
    private uint _blobId;
    private Vector2 _size;
    private float _area;
    
    private Tuio11Processor _processor;
    
    [SetUp]
    public void Init()
    {
        _sId = uint.MaxValue;
        _symbolId = uint.MaxValue;
        _position = new Vector2(2.345f, 54.123f);
        _angle = float.MaxValue;
        _velocity = new Vector2(1.23f, 5.1123f);
        _rotationSpeed = float.MaxValue;
        _acceleration = float.MaxValue;
        _rotationAcceleration = float.MaxValue;
        _sourceName = "test-source";
        _processor = new Tuio11Processor();
        _frameId = 5;
        _cursorId = uint.MaxValue;
        _blobId = uint.MaxValue;
        _size = new Vector2(12.34f, 432.12f);
        _area = float.MaxValue;
        TuioTime.Init();
    }

    private OSCMessage SetSourceMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("source");
        message.Append(_sourceName);
        return message;
    }

    private OSCMessage SetAliveMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("alive");
        message.Append((int)_sId);
        return message;
    }

    private OSCMessage SetFseqMessage(string tuioAddress)
    {
        var message = new OSCMessage(tuioAddress);
        message.Append("fseq");
        message.Append((int)_frameId);
        return message;
    }
    
    [Test]
    public void Test_Tuio11_Cursor_OSC_Receive()
    {
        const string TuioAddress = "/tuio/2Dcur";
        var tuioObject = new Tuio11Cursor(TuioTime.GetSystemTime(), _sId, _cursorId, _position, _velocity, _acceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioObject.OscMessage;
        bundle.Append(SetSourceMessage(TuioAddress));
        bundle.Append(SetAliveMessage(TuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(TuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetCursorMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        _processor.On2Dcur(this, targetSourceMessage);
        _processor.On2Dcur(this, targetAliveMessage);
        _processor.On2Dcur(this, targetCursorMessage);
        _processor.On2Dcur(this, targetFseqMessage);

        Assert.That(_processor.GetTuioCursors(), Has.Count.EqualTo(1));

        var targetCursor = _processor.GetTuioCursors()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetCursor.SessionId, Is.EqualTo(_sId));
            Assert.That(targetCursor.Position, Is.EqualTo(_position));
            Assert.That(targetCursor.Velocity, Is.EqualTo(_velocity));
            Assert.That(targetCursor.Acceleration, Is.EqualTo(_acceleration));
        });
    }

    [Test]
    public void Test_Tuio11_Object_OSC_Receive()
    {
        const string TuioAddress = "/tuio/2Dobj";
        var tuioObject = new Tuio11Object(TuioTime.GetSystemTime(), _sId, _symbolId, _position, _angle, _velocity,
            _rotationSpeed, _acceleration, _rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioObject.OscMessage;
        bundle.Append(SetSourceMessage(TuioAddress));
        bundle.Append(SetAliveMessage(TuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(TuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetObjectMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        _processor.On2Dobj(this, targetSourceMessage);
        _processor.On2Dobj(this, targetAliveMessage);
        _processor.On2Dobj(this, targetObjectMessage);
        _processor.On2Dobj(this, targetFseqMessage);

        Assert.That(_processor.GetTuioObjects(), Has.Count.EqualTo(1));

        var targetObject = _processor.GetTuioObjects()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetObject.SessionId, Is.EqualTo(_sId));
            Assert.That(targetObject.SymbolId, Is.EqualTo(_symbolId));
            Assert.That(targetObject.Position, Is.EqualTo(_position));
            Assert.That(targetObject.Angle, Is.EqualTo(_angle));
            Assert.That(targetObject.Velocity, Is.EqualTo(_velocity));
            Assert.That(targetObject.RotationSpeed, Is.EqualTo(_rotationSpeed));
            Assert.That(targetObject.Acceleration, Is.EqualTo(_acceleration));
            Assert.That(targetObject.RotationAcceleration, Is.EqualTo(_rotationAcceleration));
        });
    }

    [Test]
    public void Test_Tuio11_Blob_OSC_Receive()
    {
        const string TuioAddress = "/tuio/2Dblb";
        var tuioBlob = new Tuio11Blob(TuioTime.GetSystemTime(), _sId, _blobId, _position, _angle, _size, _area, _velocity, _rotationSpeed, _acceleration, _rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = tuioBlob.OscMessage;
        bundle.Append(SetSourceMessage(TuioAddress));
        bundle.Append(SetAliveMessage(TuioAddress));
        bundle.Append(oscMessage);
        bundle.Append(SetFseqMessage(TuioAddress));
        
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetSourceMessage = (OSCMessage)packet.Values[0];
        var targetAliveMessage = (OSCMessage)packet.Values[1];
        var targetBlobMessage = (OSCMessage)packet.Values[2];
        var targetFseqMessage = (OSCMessage)packet.Values[3];

        _processor.On2Dblb(this, targetSourceMessage);
        _processor.On2Dblb(this, targetAliveMessage);
        _processor.On2Dblb(this, targetBlobMessage);
        _processor.On2Dblb(this, targetFseqMessage);

        Assert.That(_processor.GetTuioBlobs(), Has.Count.EqualTo(1));

        var targetBlob = _processor.GetTuioBlobs()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetBlob.SessionId, Is.EqualTo(_sId));
            Assert.That(targetBlob.Position, Is.EqualTo(_position));
            Assert.That(targetBlob.Angle, Is.EqualTo(_angle));
            Assert.That(targetBlob.Size, Is.EqualTo(_size));
            Assert.That(targetBlob.Area, Is.EqualTo(_area));
            Assert.That(targetBlob.Velocity, Is.EqualTo(_velocity));
            Assert.That(targetBlob.RotationSpeed, Is.EqualTo(_rotationSpeed));
            Assert.That(targetBlob.Acceleration, Is.EqualTo(_acceleration));
            Assert.That(targetBlob.RotationAcceleration, Is.EqualTo(_rotationAcceleration));
        });
    }
}