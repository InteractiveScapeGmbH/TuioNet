using System;
using TuioNet.Common;

namespace TuioNet.Tuio11;

public class Tuio11Dispatcher : ITuioDispatcher
{
    private Tuio11Processor? _processor;

    public event EventHandler<Tuio11Cursor>? OnCursorAdd; 
    public event EventHandler<Tuio11Cursor>? OnCursorUpdate; 
    public event EventHandler<Tuio11Cursor>? OnCursorRemove;
    
    public event EventHandler<Tuio11Object>? OnObjectAdd; 
    public event EventHandler<Tuio11Object>? OnObjectUpdate; 
    public event EventHandler<Tuio11Object>? OnObjectRemove; 
    
    public event EventHandler<Tuio11Blob>? OnBlobAdd; 
    public event EventHandler<Tuio11Blob>? OnBlobUpdate;
    public event EventHandler<Tuio11Blob>? OnBlobRemove;

    public event EventHandler<TuioTime>? OnRefresh; 
    
    public void SetupProcessor(TuioClient tuioClient)
    {
        _processor = new Tuio11Processor(tuioClient);
    }

    public void RegisterCallbacks()
    {
        if (_processor == null) return;
        _processor.OnCursorAdded += AddCursor;
        _processor.OnCursorUpdated += UpdateCursor;
        _processor.OnCursorRemoved += RemoveCursor;

        _processor.OnObjectAdded += AddObject;
        _processor.OnObjectUpdated += UpdateObject;
        _processor.OnObjectRemoved += RemoveObject;

        _processor.OnBlobAdded += AddBlob;
        _processor.OnBlobUpdated += UpdateBlob;
        _processor.OnBlobRemoved += RemoveBlob;

        _processor.OnRefreshed += Refresh;
    }
    
    public void UnregisterCallbacks()
    {
        if (_processor == null) return;
        _processor.OnCursorAdded -= AddCursor;
        _processor.OnCursorUpdated -= UpdateCursor;
        _processor.OnCursorRemoved -= RemoveCursor;

        _processor.OnObjectAdded -= AddObject;
        _processor.OnObjectUpdated -= UpdateObject;
        _processor.OnObjectRemoved -= RemoveObject;

        _processor.OnBlobAdded -= AddBlob;
        _processor.OnBlobUpdated -= UpdateBlob;
        _processor.OnBlobRemoved -= RemoveBlob;

        _processor.OnRefreshed -= Refresh;
    }
    
    private void AddCursor(object? sender, Tuio11Cursor cursor)
    {
        OnCursorAdd?.Invoke(sender, cursor);
    }
    
    private void UpdateCursor(object? sender, Tuio11Cursor cursor)
    {
        OnCursorUpdate?.Invoke(sender, cursor);
    }
    
    private void RemoveCursor(object? sender, Tuio11Cursor cursor)
    {
        OnCursorRemove?.Invoke(sender, cursor);
    }
    
    private void AddObject(object? sender, Tuio11Object tuioObject)
    {
        OnObjectAdd?.Invoke(sender, tuioObject);
    }
    
    private void UpdateObject(object? sender, Tuio11Object tuioObject)
    {
        OnObjectUpdate?.Invoke(sender, tuioObject);
    }
    
    private void RemoveObject(object? sender, Tuio11Object tuioObject)
    {
        OnObjectRemove?.Invoke(sender, tuioObject);
    }

    private void AddBlob(object? sender, Tuio11Blob blob)
    {
        OnBlobAdd?.Invoke(sender, blob);
    }
    
    private void UpdateBlob(object? sender, Tuio11Blob blob)
    {
        OnBlobUpdate?.Invoke(sender, blob);
    }
    
    private void RemoveBlob(object? sender, Tuio11Blob blob)
    {
        OnBlobRemove?.Invoke(sender, blob);
    }

    private void Refresh(object? sender, TuioTime tuioTime)
    {
        OnRefresh?.Invoke(sender, tuioTime);
    }
}