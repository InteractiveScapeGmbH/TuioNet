using System.Numerics;
using NUnit.Framework;
using OSC.NET;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio20;

namespace TuioNet.Tests.Tuio20;

[TestFixture]
public class Test_Tuio20_Send
{
    private string sourceName;
    private Vector2 resolution;
    
    [SetUp]
    public void Init()
    {
        sourceName = "test-source";
        resolution = new Vector2(800, 600);
    }
    
    [Test]
    public void Test_Token_Send()
    {
        Tuio20Manager manager = new Tuio20Manager(sourceName, resolution);
        Tuio20Object container = new Tuio20Object(TuioTime.GetSystemTime(), 1234);
        Tuio20Token token = new Tuio20Token(TuioTime.GetSystemTime(), container, 456, 987, Vector2.Zero, 0f,
            Vector2.One, 0f, 0f, 0f);
        manager.AddEntity(token);

        var bundle = manager.FrameBundle;
        var byteBundle = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteBundle);
        var targetFrameMessage = (OSCMessage)packet.Values[0];
        var targetTokenMessage = (OSCMessage)packet.Values[1];
        var targetAliveMessage = (OSCMessage)packet.Values[2];

        var processor = new Tuio20Processor();
        processor.OnFrm(this, targetFrameMessage);
        processor.OnOther(this, targetTokenMessage);
        processor.OnAlv(this, targetAliveMessage);

        Assert.AreEqual(1, processor.GetTuioTokenList().Count);

        var receivedToken = processor.GetTuioTokenList()[0];
        Assert.AreEqual(resolution, processor.SensorDimension);
        Assert.AreEqual(sourceName, processor.Source);
        Assert.AreEqual(1234, receivedToken.SessionId);
    }
}