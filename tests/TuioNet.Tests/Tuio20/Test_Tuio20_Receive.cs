using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.Tuio20;

namespace TuioNet.Tests.Tuio20;

[TestFixture]
public class TestTuio20Receive
{
    private uint _sId;
    private uint _typeUserId;
    private uint _componentId;
    private Tuio20Object _container;
    private Vector2 _position;
    private float _angle;
    private Vector2 _velocity;
    private float _rotationSpeed;
    private float _acceleration;
    private float _rotationAcceleration;
    private float _shear;
    private float _radius;
    private float _pressure;
    private float _pressureSpeed;
    private float _pressureAcceleration;
    private Vector2 _size;
    private float _area;
    private string _group;
    private string _data;

    private Tuio20Processor _processor;
    private OSCMessage _frameMessage;
    private OSCMessage _aliveMessage;
    
    [SetUp]
    public void Init()
    {
        _sId = uint.MaxValue;
        _typeUserId = uint.MaxValue;
        _componentId = uint.MaxValue;
        _container = new Tuio20Object(TuioTime.GetSystemTime(), _sId);
        _position = new Vector2(2.3f, 6.3f);
        _angle = float.MaxValue;
        _velocity = new Vector2(8.3f, 4.3f);
        _rotationSpeed = float.MaxValue;
        _acceleration = float.MaxValue;
        _rotationAcceleration = float.MaxValue;
        _shear = float.MaxValue;
        _radius = float.MaxValue;
        _pressure = float.MaxValue;
        _pressureSpeed = float.MaxValue;
        _pressureAcceleration = float.MaxValue;
        _size = new Vector2(2.45f, 7.34f);
        _area = float.MaxValue;
        _group = "test-group";
        _data = "test-data";
        
        _processor = new Tuio20Processor();
        _frameMessage = SetFrameMessage(5);
        _aliveMessage = SetAliveMessage(_sId);
    }

    private OSCMessage SetAliveMessage(uint sId)
    {
        var message = new OSCMessage("/tuio2/alv"); 
        message.Append((int)sId);
        return message;
    }

    private OSCMessage SetFrameMessage(uint frameId)
    {
        var message = new OSCMessage("/tuio2/frm");
        message.Append((int)frameId);
        message.Append(new OscTimeTag(DateTime.Now));
        message.Append(Int32.MaxValue);
        message.Append("Test");
        return message;
    }
    
    [Test]
    public void Test_Tuio20_Token_OSC_Receive()
    {
        var token = new Tuio20Token(TuioTime.GetSystemTime(), _container, _typeUserId, _componentId, _position, _angle,
            _velocity, _rotationSpeed, _acceleration, _rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = token.OscMessage;
        bundle.Append(_frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(_aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        _processor.OnFrm(this, targetFrameMessage);
        _processor.OnOther(this, targetTokenMessage);
        _processor.OnAlv(this, targetAliveMessage);

        Assert.That(_processor.GetTuioTokenList(), Has.Count.EqualTo(1));
        
        var targetToken = _processor.GetTuioTokenList()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetToken.SessionId, Is.EqualTo(_sId));
            Assert.That(targetToken.TypeUserId, Is.EqualTo(_typeUserId));
            Assert.That(targetToken.ComponentId, Is.EqualTo(_componentId));
            Assert.That(targetToken.Position.X, Is.EqualTo(_position.X));
            Assert.That(targetToken.Position.Y, Is.EqualTo(_position.Y));
            Assert.That(targetToken.Angle, Is.EqualTo(_angle));
            Assert.That(targetToken.Velocity.X, Is.EqualTo(_velocity.X));
            Assert.That(targetToken.Velocity.Y, Is.EqualTo(_velocity.Y));
            Assert.That(targetToken.RotationSpeed, Is.EqualTo(_rotationSpeed));
            Assert.That(targetToken.Acceleration, Is.EqualTo(_acceleration));
            Assert.That(targetToken.RotationAcceleration, Is.EqualTo(_rotationAcceleration));
        });
    }

    [Test]
    public void Test_Tuio20_Pointer_OSC_Receive()
    {
        var pointer = new Tuio20Pointer(TuioTime.GetSystemTime(), _container, _typeUserId, _componentId, _position, _angle, _shear, _radius, _pressure, _velocity, _pressureSpeed, _acceleration, _pressureAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = pointer.OscMessage;
        bundle.Append(_frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(_aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        _processor.OnFrm(this, targetFrameMessage);
        _processor.OnOther(this, targetTokenMessage);
        _processor.OnAlv(this, targetAliveMessage);
        
        Assert.That(_processor.GetTuioPointerList(), Has.Count.EqualTo(1));
        
        var targetPointer = _processor.GetTuioPointerList()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetPointer.SessionId, Is.EqualTo(_sId));
            Assert.That(targetPointer.TypeUserId, Is.EqualTo(_typeUserId));
            Assert.That(targetPointer.ComponentId, Is.EqualTo(_componentId));
            Assert.That(targetPointer.Position.X, Is.EqualTo(_position.X));
            Assert.That(targetPointer.Position.Y, Is.EqualTo(_position.Y));
            Assert.That(targetPointer.Angle, Is.EqualTo(_angle));
            Assert.That(targetPointer.Shear, Is.EqualTo(_shear));
            Assert.That(targetPointer.Radius, Is.EqualTo(_radius));
            Assert.That(targetPointer.Pressure, Is.EqualTo(_pressure));
            Assert.That(targetPointer.Velocity.X, Is.EqualTo(_velocity.X));
            Assert.That(targetPointer.Velocity.Y, Is.EqualTo(_velocity.Y));
            Assert.That(targetPointer.PressureSpeed, Is.EqualTo(_pressureSpeed));
            Assert.That(targetPointer.Acceleration, Is.EqualTo(_acceleration));
            Assert.That(targetPointer.PressureAcceleration, Is.EqualTo(_pressureAcceleration));
        });
    }

    [Test]
    public void Test_Tuio20_Bounds_OSC_Receive()
    {
        var bounds = new Tuio20Bounds(TuioTime.GetSystemTime(), _container, _position, _angle, _size, _area, _velocity, _rotationSpeed, _acceleration, _rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = bounds.OscMessage;
        bundle.Append(_frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(_aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        _processor.OnFrm(this, targetFrameMessage);
        _processor.OnOther(this, targetTokenMessage);
        _processor.OnAlv(this, targetAliveMessage);
        
        Assert.That(_processor.GetTuioBoundsList(), Has.Count.EqualTo(1));
        
        var targetBounds = _processor.GetTuioBoundsList()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetBounds.SessionId, Is.EqualTo(_sId));
            Assert.That(targetBounds.Position.X, Is.EqualTo(_position.X));
            Assert.That(targetBounds.Position.Y, Is.EqualTo(_position.Y));
            Assert.That(targetBounds.Angle, Is.EqualTo(_angle));
            Assert.That(targetBounds.Size.X, Is.EqualTo(_size.X));
            Assert.That(targetBounds.Size.Y, Is.EqualTo(_size.Y));
            Assert.That(targetBounds.Area, Is.EqualTo(_area));
            Assert.That(targetBounds.Velocity.X, Is.EqualTo(_velocity.X));
            Assert.That(targetBounds.Velocity.Y, Is.EqualTo(_velocity.Y));
            Assert.That(targetBounds.RotationSpeed, Is.EqualTo(_rotationSpeed));
            Assert.That(targetBounds.Acceleration, Is.EqualTo(_acceleration));
            Assert.That(targetBounds.RotationAcceleration, Is.EqualTo(_rotationAcceleration));
        });
    }

    [Test]
    public void Test_Tuio20_Symbol_OSC_Receive()
    {
        var symbol = new Tuio20Symbol(TuioTime.GetSystemTime(), _container, _typeUserId, _componentId, _group, _data);
        var bundle = new OSCBundle();
        var oscMessage = symbol.OscMessage;
        bundle.Append(_frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(_aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        _processor.OnFrm(this, targetFrameMessage);
        _processor.OnOther(this, targetTokenMessage);
        _processor.OnAlv(this, targetAliveMessage);
        
        Assert.That(_processor.GetTuioSymbolList(), Has.Count.EqualTo(1));
        
        var targetSymbol = _processor.GetTuioSymbolList()[0];
        Assert.Multiple(() =>
        {
            Assert.That(targetSymbol.SessionId, Is.EqualTo(_sId));
            Assert.That(targetSymbol.TypeUserId, Is.EqualTo(_typeUserId));
            Assert.That(targetSymbol.ComponentId, Is.EqualTo(_componentId));
            Assert.That(targetSymbol.Group, Is.EqualTo(_group));
            Assert.That(targetSymbol.Data, Is.EqualTo(_data));
        });
    }
}