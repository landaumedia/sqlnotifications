namespace LandauMedia.Storage
{
    public class DatabaseVersionStorage : IVersionStorage
    {
        public void Store(string key, ulong version)
        {
            throw new System.NotImplementedException();
        }

        public ulong Load(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}