using NUnit.Framework;
using OSC.NET;

namespace TuioNet.Tests.OSC;

[TestFixture]
public class Test_OSC_Bundle
{
    [Test]
    public void OSC_frame_bundle()
    {
        var bundle = new OSCBundle();
        var sourceMessage = new OSCMessage("/tuio2/frm");
        uint frame_id = UInt32.MaxValue;
        int frame_id_cast = (int)frame_id;
        sourceMessage.Append(frame_id_cast);
        sourceMessage.Append(new OscTimeTag(DateTime.Now));
        sourceMessage.Append(36);
        sourceMessage.Append("Test");
        bundle.Append(sourceMessage);

        var byteMessage = bundle.BinaryData;
        var packet = OSCPacket.Unpack(byteMessage);

        var targetMessage = (OSCMessage)packet.Values[0];
        Assert.AreEqual((uint)(int)targetMessage.Values[0], frame_id);
    }
}