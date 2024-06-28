using System;

namespace TuioNet.Common;

public class MessageEventArgs : EventArgs
{
    public byte[] Buffer { get; set; }
    public int Length { get; set; }
}