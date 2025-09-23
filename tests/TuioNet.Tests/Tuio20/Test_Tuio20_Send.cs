using System.Numerics;
using NUnit.Framework;
using TuioNet.Client.Tuio20;
using TuioNet.Common;
using TuioNet.OSC;
using TuioNet.Server;
using TuioNet.Tuio20;

namespace TuioNet.Tests.Tuio20;

[TestFixture]
public class TestTuio20Send
{
    private string _sourceName;
    private Vector2 _resolution;
    
    [SetUp]
    public void Init()
    {
        _sourceName = "test-source";
        _resolution = new Vector2(800, 600);
    }
    
    [Test]
    public void Test_Token_Send()
    {
        var manager = new Tuio20Manager(_sourceName, _resolution);
        var container = new Tuio20Object(TuioTime.GetSystemTime(), 1234);
        var token = new Tuio20Token(TuioTime.GetSystemTime(), container, 456, 987, Vector2.Zero, 0f,
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

        Assert.That(processor.GetTuioTokenList(), Has.Count.EqualTo(1));

        var receivedToken = processor.GetTuioTokenList()[0];
        Assert.Multiple(() =>
        {
            Assert.That(processor.SensorDimension, Is.EqualTo(_resolution));
            Assert.That(processor.Source, Is.EqualTo(_sourceName));
            Assert.That(receivedToken.SessionId, Is.EqualTo(1234));
        });
    }
}