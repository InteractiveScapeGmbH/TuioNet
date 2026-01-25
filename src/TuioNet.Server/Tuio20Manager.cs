using System.Numerics;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Server
{
    
public class Tuio20Manager : ITuioManager
{
    private readonly Tuio20Repository _repository;

    private readonly OSCBundle[] _frameBundle = new OSCBundle[1];
    private uint _frameId;

    public Tuio20Manager(string sourceName, Vector2 screenResolution)
    {
        _repository = new Tuio20Repository(sourceName, screenResolution);
    }

    public uint CurrentSessionId { get; private set; }

    public OSCBundle[] FrameBundles
    {
        get
        {
            UpdateFrameBundle();
            return _frameBundle;
        }
    }

    public void Update()
    {
        _frameId += 1;
        _repository.Update(_frameId);
    }

    public void Quit()
    {
        _repository.Clear();
        Update();
    }

    public void AddEntity(ITuioEntity entity)
    {
        _repository.AddEntity(entity);
        CurrentSessionId++;
    }

    public void RemoveEntity(ITuioEntity entity)
    {
        _repository.RemoveEntity(entity);
    }

    private void UpdateFrameBundle()
    {
        _frameBundle[0] = _repository.Bundle();
    }
}
}
