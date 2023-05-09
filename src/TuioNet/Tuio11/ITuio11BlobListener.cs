namespace TuioNet.Tuio11
{
    public interface ITuio11BlobListener
    {
        public void AddBlob(Tuio11Blob blob);
        public void UpdateBlob(Tuio11Blob blob);
        public void RemoveBlob(Tuio11Blob blob);
    }
}