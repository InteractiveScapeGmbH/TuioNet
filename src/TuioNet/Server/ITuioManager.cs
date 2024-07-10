﻿using OSC.NET;

namespace TuioNet.Server;

public interface ITuioManager
{
    public OSCBundle FrameBundle { get; }
    public void Update();
    public void Quit();
}