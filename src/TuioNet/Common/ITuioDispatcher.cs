namespace TuioNet.Common;

public interface ITuioDispatcher
{
    public void SetupProcessor(TuioClient tuioClient);
    public void RegisterCallbacks();
    public void UnregisterCallbacks();
}