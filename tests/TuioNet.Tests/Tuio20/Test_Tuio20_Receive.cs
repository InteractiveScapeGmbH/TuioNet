using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.Tuio20;

namespace TuioNet.Tests.Tuio20;

[TestFixture]
public class Test_Tuio20_Receive
{
    private uint sId;
    private uint typeUserId;
    private uint componentId;
    private Tuio20Object container;
    private Vector2 position;
    private float angle;
    private Vector2 velocity;
    private float rotationSpeed;
    private float acceleration;
    private float rotationAcceleration;
    private float shear;
    private float radius;
    private float pressure;
    private float pressureSpeed;
    private float pressureAcceleration;
    private Vector2 size;
    private float area;
    private string group;
    private string data;

    private Tuio20Processor processor;
    private OSCMessage frameMessage;
    private OSCMessage aliveMessage;
    
    [SetUp]
    public void Init()
    {
        sId = UInt32.MaxValue;
        typeUserId = UInt32.MaxValue;
        componentId = UInt32.MaxValue;
        container = new Tuio20Object(TuioTime.GetSystemTime(), sId);
        position = new Vector2(2.3f, 6.3f);
        angle = float.MaxValue;
        velocity = new Vector2(8.3f, 4.3f);
        rotationSpeed = float.MaxValue;
        acceleration = float.MaxValue;
        rotationAcceleration = float.MaxValue;
        shear = float.MaxValue;
        radius = float.MaxValue;
        pressure = float.MaxValue;
        pressureSpeed = float.MaxValue;
        pressureAcceleration = float.MaxValue;
        size = new Vector2(2.45f, 7.34f);
        area = float.MaxValue;
        group = "test-group";
        data = "test-data";
        
        processor = new Tuio20Processor();
        frameMessage = SetFrameMessage(5);
        aliveMessage = SetAliveMessage(sId);
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
        var token = new Tuio20Token(TuioTime.GetSystemTime(), container, typeUserId, componentId, position, angle,
            velocity, rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = token.OscMessage;
        bundle.Append(frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        processor.OnFrm(this, targetFrameMessage);
        processor.OnOther(this, targetTokenMessage);
        processor.OnAlv(this, targetAliveMessage);

        Assert.AreEqual(1, processor.GetTuioTokenList().Count);
        
        var targetToken = processor.GetTuioTokenList()[0];
        Assert.AreEqual(sId, targetToken.SessionId);
        Assert.AreEqual(typeUserId, targetToken.TypeUserId);
        Assert.AreEqual(componentId, targetToken.ComponentId);
        Assert.AreEqual(position.X, targetToken.Position.X);
        Assert.AreEqual(position.Y, targetToken.Position.Y);
        Assert.AreEqual(angle, targetToken.Angle);
        Assert.AreEqual(velocity.X, targetToken.Velocity.X);
        Assert.AreEqual(velocity.Y, targetToken.Velocity.Y);
        Assert.AreEqual(rotationSpeed, targetToken.RotationSpeed);
        Assert.AreEqual(acceleration, targetToken.Acceleration);
        Assert.AreEqual(rotationAcceleration, targetToken.RotationAcceleration);
    }
    
    [Test]
    public void Test_Tuio20_Pointer_OSC_Receive()
    {
        var pointer = new Tuio20Pointer(TuioTime.GetSystemTime(), container, typeUserId, componentId, position, angle, shear, radius, pressure, velocity, pressureSpeed, acceleration, pressureAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = pointer.OscMessage;
        bundle.Append(frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        processor.OnFrm(this, targetFrameMessage);
        processor.OnOther(this, targetTokenMessage);
        processor.OnAlv(this, targetAliveMessage);
        
        Assert.AreEqual(1, processor.GetTuioPointerList().Count);
        
        var targetPointer = processor.GetTuioPointerList()[0];
        Assert.AreEqual(sId, targetPointer.SessionId);
        Assert.AreEqual(typeUserId, targetPointer.TypeUserId);
        Assert.AreEqual(componentId, targetPointer.ComponentId);
        Assert.AreEqual(position.X, targetPointer.Position.X);
        Assert.AreEqual(position.Y, targetPointer.Position.Y);
        Assert.AreEqual(angle, targetPointer.Angle);
        Assert.AreEqual(shear, targetPointer.Shear);
        Assert.AreEqual(radius, targetPointer.Radius);
        Assert.AreEqual(pressure, targetPointer.Pressure);
        Assert.AreEqual(velocity.X, targetPointer.Velocity.X);
        Assert.AreEqual(velocity.Y, targetPointer.Velocity.Y);
        Assert.AreEqual(pressureSpeed, targetPointer.PressureSpeed);
        Assert.AreEqual(acceleration, targetPointer.Acceleration);
        Assert.AreEqual(pressureAcceleration, targetPointer.PressureAcceleration);
    }
    
    [Test]
    public void Test_Tuio20_Bounds_OSC_Receive()
    {
        var bounds = new Tuio20Bounds(TuioTime.GetSystemTime(), container, position, angle, size, area, velocity, rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = bounds.OscMessage;
        bundle.Append(frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        processor.OnFrm(this, targetFrameMessage);
        processor.OnOther(this, targetTokenMessage);
        processor.OnAlv(this, targetAliveMessage);
        
        Assert.AreEqual(1, processor.GetTuioBoundsList().Count);
        
        var targetBounds = processor.GetTuioBoundsList()[0];
        Assert.AreEqual(sId, targetBounds.SessionId);
        Assert.AreEqual(position.X, targetBounds.Position.X);
        Assert.AreEqual(position.Y, targetBounds.Position.Y);
        Assert.AreEqual(angle, targetBounds.Angle);
        Assert.AreEqual(size.X, targetBounds.Size.X);
        Assert.AreEqual(size.Y, targetBounds.Size.Y);
        Assert.AreEqual(area, targetBounds.Area);
        Assert.AreEqual(velocity.X, targetBounds.Velocity.X);
        Assert.AreEqual(velocity.Y, targetBounds.Velocity.Y);
        Assert.AreEqual(rotationSpeed, targetBounds.RotationSpeed);
        Assert.AreEqual(acceleration, targetBounds.Acceleration);
        Assert.AreEqual(rotationAcceleration, targetBounds.RotationAcceleration);
    }

    [Test]
    public void Test_Tuio20_Symbol_OSC_Receive()
    {
        var symbol = new Tuio20Symbol(TuioTime.GetSystemTime(), container, typeUserId, componentId, group, data);
        var bundle = new OSCBundle();
        var oscMessage = symbol.OscMessage;
        bundle.Append(frameMessage);
        bundle.Append(oscMessage);
        bundle.Append(aliveMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];
        
        processor.OnFrm(this, targetFrameMessage);
        processor.OnOther(this, targetTokenMessage);
        processor.OnAlv(this, targetAliveMessage);
        
        Assert.AreEqual(1, processor.GetTuioSymbolList().Count);
        
        var targetSymbol = processor.GetTuioSymbolList()[0];
        Assert.AreEqual(sId, targetSymbol.SessionId);
        Assert.AreEqual(typeUserId, targetSymbol.TypeUserId);
        Assert.AreEqual(componentId, targetSymbol.ComponentId);
        Assert.AreEqual(group, targetSymbol.Group);
        Assert.AreEqual(data, targetSymbol.Data);
    }
}