namespace LandauMedia.Storage
{
    public interface IVersionStorage
    {
        void Store(string key, ulong version);

        ulong Load(string key);
    }
}