namespace LandauMedia.Infrastructure
{
    public interface IPerformanceCounter
    {
        // command
        void Inc(string name);
    }
}