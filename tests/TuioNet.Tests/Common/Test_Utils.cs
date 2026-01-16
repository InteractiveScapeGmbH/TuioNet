using System.Numerics;
using NUnit.Framework;
using TuioNet.Common;

namespace TuioNet.Tests.Common;

public class Test_Utils
{
    [Test]
    public void Test_Pack_Dimension()
    {
        var dimension = new Vector2(800, 600);
        var packed = Utils.FromDimension(dimension);
        var unpacked = Utils.ToDimension(packed);
        Assert.That(unpacked, Is.EqualTo(dimension));
    }
}