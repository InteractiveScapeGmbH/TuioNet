using NUnit.Framework;
using OSC.NET;

namespace TuioNet.Tests.OSC;

[TestFixture]
public class Test_OSC_Message
{
    private OSCMessage _message;
    [SetUp]
    public void Init()
    {
        _message = new OSCMessage("test");
    }
    
    [Test]
    public void OSC_int()
    {
        int a = 10;
        _message.Append(a);
        var v = (int)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_int_min()
    {
        int a = Int32.MinValue;
        _message.Append(a);
        var v = (int)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_int_max()
    {
        int a = Int32.MaxValue;
        _message.Append(a);
        var v = (int)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_uint()
    {
        uint a = 10;
        _message.Append(a);
        var v = (uint)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_uint_min()
    {
        uint a = UInt32.MinValue;
        _message.Append(a);
        var v = (uint)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_uint_max()
    {
        uint a = UInt32.MaxValue;
        _message.Append(a);
        var v = (uint)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_float()
    {
        float a = 1.234f;
        _message.Append(a);
        var v = (float)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_float_min()
    {
        float a = Single.MinValue;
        _message.Append(a);
        var v = (float)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_float_max()
    {
        float a = Single.MaxValue;
        _message.Append(a);
        var v = (float)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_double()
    {
        double a = 1.234;
        _message.Append(a);
        var v = (double)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_double_min()
    {
        double a = Double.MinValue;
        _message.Append(a);
        var v = (double)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_double_max()
    {
        double a = Double.MaxValue;
        _message.Append(a);
        var v = (double)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_long()
    {
        long a = 123456;
        _message.Append(a);
        var v = (long)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_long_min()
    {
        long a = long.MinValue;
        _message.Append(a);
        var v = (long)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_long_max()
    {
        long a = long.MaxValue;
        _message.Append(a);
        var v = (long)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_string()
    {
        string a = "Hello world";
        _message.Append(a);
        var v = (string)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
    [Test]
    public void OSC_time()
    {
        OscTimeTag a = new OscTimeTag(DateTime.Now);
        _message.Append(a);
        var v = (OscTimeTag)_message.Values[0];
        Assert.AreEqual(a.DateTime, v.DateTime);
    }
    
    [Test]
    public void OSC_char()
    {
        char a = 'a';
        _message.Append(a);
        var v = (char)_message.Values[0];
        Assert.AreEqual(a, v);
    }
    
}