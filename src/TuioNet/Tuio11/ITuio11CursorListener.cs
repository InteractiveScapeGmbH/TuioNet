namespace TuioNet.Tuio11
{
    public interface ITuio11CursorListener
    {
        public void AddCursor(Tuio11Cursor cursor);
        public void UpdateCursor(Tuio11Cursor cursor);
        public void RemoveCursor(Tuio11Cursor cursor);
    }
}