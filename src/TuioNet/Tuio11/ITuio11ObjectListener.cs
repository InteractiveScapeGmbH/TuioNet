namespace TuioNet.Tuio11
{
    public interface ITuio11ObjectListener
    {
        public void AddObject(Tuio11Object tuioObject);
        public void UpdateObject(Tuio11Object tuioObject);
        public void RemoveObject(Tuio11Object tuioObject);
    }
}