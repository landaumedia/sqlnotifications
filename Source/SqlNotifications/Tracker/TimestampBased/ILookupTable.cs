namespace LandauMedia.Tracker.TimestampBased
{
    public interface ILookupTable
    {
        void Add(object key);

        bool Contains(object key);
    }
}