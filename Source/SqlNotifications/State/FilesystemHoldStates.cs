using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LandauMedia.State
{
    public class FilesystemHoldStates : IHoldStates
    {
        readonly DirectoryInfo _dataStateDirectory;

        public FilesystemHoldStates(DirectoryInfo dataStateDirectory)
        {
            _dataStateDirectory = dataStateDirectory;
        }

        public void Put(string id, State state)
        {
            var typeDirectory = EnsureDirectoryExistAndGet(state.StateType);

            var dataFile = typeDirectory.EnumerateFiles().First(t => t.Name == id);

            using (var writer = new StreamWriter(dataFile.FullName, false, Encoding.UTF8))
            {
                writer.Write(JsonConvert.SerializeObject(state));
            }
        }

        DirectoryInfo EnsureDirectoryExistAndGet(string type)
        {
            return _dataStateDirectory.EnumerateDirectories().FirstOrDefault(t => t.Name == type) ??
                   _dataStateDirectory.CreateSubdirectory(type);
        }

        public State GetForTypeOrNull(string id, string type)
        {
            var typeDirectory = EnsureDirectoryExistAndGet(type);
            var file = typeDirectory.EnumerateFiles().FirstOrDefault(t => t.Name == type);

            if (file == null)
                return null;

            using (var reader = new StreamReader(file.FullName, Encoding.Default))
                return JsonConvert.DeserializeObject<State>(reader.ReadToEnd());
        }
    }
}