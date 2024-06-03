using System;
using TuioNet.Common;

namespace TuioNet.Tuio20;

public class Tuio20Dispatcher : ITuioDispatcher
{
    public event EventHandler<Tuio20Object>? OnObjectAdd; 
    public event EventHandler<Tuio20Object>? OnObjectUpdate; 
    public event EventHandler<Tuio20Object>? OnObjectRemove;
    public event EventHandler<TuioTime>? OnRefresh; 
    
    private Tuio20Processor? _processor;
    
    public void SetupProcessor(TuioClient tuioClient)
    {
        _processor = new Tuio20Processor(tuioClient);
    }

    public void RegisterCallbacks()
    {
        if (_processor == null) return;
        _processor.OnObjectAdded += AddObject;
        _processor.OnObjectUpdated += UpdateObject;
        _processor.OnObjectRemoved += RemoveObject;
        _processor.OnRefreshed += Refresh;
    }

    public void UnregisterCallbacks()
    {
        if (_processor == null) return;
        _processor.OnObjectAdded -= AddObject;
        _processor.OnObjectUpdated -= UpdateObject;
        _processor.OnObjectRemoved -= RemoveObject;
        _processor.OnRefreshed -= Refresh;
    }

    private void AddObject(object? sender, Tuio20Object tuioObject)
    {
        OnObjectAdd?.Invoke(sender, tuioObject);
    }
    
    private void UpdateObject(object? sender, Tuio20Object tuioObject)
    {
        OnObjectUpdate?.Invoke(sender, tuioObject);
    }
    
    private void RemoveObject(object? sender, Tuio20Object tuioObject)
    {
        OnObjectRemove?.Invoke(sender, tuioObject);
    }
    
    private void Refresh(object? sender, TuioTime tuioTime)
    {
        OnRefresh?.Invoke(sender, tuioTime);
    }
}