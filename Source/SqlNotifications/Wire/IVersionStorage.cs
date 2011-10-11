namespace LandauMedia.Wire
{
    public interface IVersionStorage
    {
        void Store(object version);

        object Load();
    }
}