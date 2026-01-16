using NUnit.Framework;
using TuioNet.Common;

namespace TuioNet.Tests.Common;

public class TuioTimeTests
{
    [Test]
    public void Add_Two_TuioTimes()
    {
        var timeA = new TuioTime(1, 15);
        var timeB = new TuioTime(2, 10);
        var result = timeA + timeB;
        Assert.That(3, Is.EqualTo(result.Seconds));
        Assert.That(25, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Add_Two_TuioTimes_With_Overflowing_Microseconds()
    {
        var timeA = new TuioTime(2, 999999);
        var timeB = new TuioTime(0, 3);
        var result = timeA + timeB;
        Assert.That(3, Is.EqualTo(result.Seconds));
        Assert.That(2, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Add_Negative_TuioTime()
    {
        var timeA = new TuioTime(3, 5);
        var timeB = new TuioTime(-2, -3);
        var result = timeA + timeB;
        Assert.That(1, Is.EqualTo(result.Seconds));
        Assert.That(2, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Add_Microseconds()
    {
        var time = new TuioTime(3, 20);
        long microsec = 500;
        var result = time + microsec;
        Assert.That(3, Is.EqualTo(result.Seconds));
        Assert.That(520, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Add_Microseconds_With_Overflow()
    {
        var time = new TuioTime(2, 999999);
        var microsec = 3;
        var result = time + microsec;
        Assert.That(3, Is.EqualTo(result.Seconds));
        Assert.That(2, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Add_Negative_Microseconds_With_Overflow()
    {
        var time = new TuioTime(2, 2);
        var microsec = -3;
        var result = time + microsec;
        Assert.That(1, Is.EqualTo(result.Seconds));
        Assert.That(999999, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Subtract_Two_TuioTimes()
    {
        var timeA = new TuioTime(3, 25);
        var timeB = new TuioTime(2, 12);
        var result = timeA - timeB;
        Assert.That(1, Is.EqualTo(result.Seconds));
        Assert.That(13, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Subtract_Two_TuioTimes_With_Overflow()
    {
        var timeA = new TuioTime(3, 3);
        var timeB = new TuioTime(1, 4);
        var result = timeA - timeB;
        Assert.That(1, Is.EqualTo(result.Seconds));
        Assert.That(999999, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Subtract_Microseconds()
    {
        var time = new TuioTime(3, 10);
        var microseconds = 5;
        var result = time - microseconds;
        Assert.That(3, Is.EqualTo(result.Seconds));
        Assert.That(5, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Subtract_Microseconds_With_Overflow()
    {
        var time = new TuioTime(3, 10);
        var microseconds = 11;
        var result = time - microseconds;
        Assert.That(2, Is.EqualTo(result.Seconds));
        Assert.That(999999, Is.EqualTo(result.Microseconds));
    }

    [Test]
    public void Are_Two_TuioTimes_Equal()
    {
        var timeA = new TuioTime(1, 123);
        var timeB = new TuioTime(1, 123);
        Assert.That(timeA.Equals(timeB), Is.EqualTo(true));
    }
}