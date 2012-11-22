namespace LandauMedia.State
{
    public interface IStatePersister
    {
        void Save(string id, State state);

        State GetForTypeOrNull(string id, string type);

    }
}