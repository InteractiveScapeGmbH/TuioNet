using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Common;
using TuioNet.Tuio20;

namespace TuioNet.Tests.Tuio20;

[TestFixture]
public class Test_Tuio20
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
    }
    
    [Test]
    public void Test_Tuio20_Token_Over_OSC()
    {

        var token = new Tuio20Token(TuioTime.GetSystemTime(), container, typeUserId, componentId, position, angle,
            velocity, rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = token.OscMessage;
        bundle.Append(oscMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetMessage = (OSCMessage)packet.Values[0];
        
        Assert.AreEqual(sId, (uint)(int)targetMessage.Values[0]);
        Assert.AreEqual(typeUserId, (uint)(int)targetMessage.Values[1]);
        Assert.AreEqual(componentId, (uint)(int)targetMessage.Values[2]);
    }
    
    [Test]
    public void Test_Tuio20_Pointer_Over_OSC()
    {
        var pointer = new Tuio20Pointer(TuioTime.GetSystemTime(), container, typeUserId, componentId, position, angle, shear, radius, pressure, velocity, pressureSpeed, acceleration, pressureAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = pointer.OscMessage;
        bundle.Append(oscMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetMessage = (OSCMessage)packet.Values[0];
        
        Assert.AreEqual(sId, (uint)(int)targetMessage.Values[0]);
        Assert.AreEqual(typeUserId, (uint)(int)targetMessage.Values[1]);
        Assert.AreEqual(componentId, (uint)(int)targetMessage.Values[2]);
    }
    
    [Test]
    public void Test_Tuio20_Bounds_Over_OSC()
    {
        var pointer = new Tuio20Bounds(TuioTime.GetSystemTime(), container, position, angle, size, area, velocity, rotationSpeed, acceleration, rotationAcceleration);
        var bundle = new OSCBundle();
        var oscMessage = pointer.OscMessage;
        bundle.Append(oscMessage);
        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);
        var targetMessage = (OSCMessage)packet.Values[0];
        
        Assert.AreEqual(sId, (uint)(int)targetMessage.Values[0]);
    }
}