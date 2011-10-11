using System.IO;
using System.Text;

namespace LandauMedia.Wire
{
    public class FilebasedVersionStorage : IVersionStorage
    {
        readonly FileInfo _storageFile;

        public FilebasedVersionStorage(string storageFile)
        {
            _storageFile = new FileInfo(storageFile);
        }

        public FilebasedVersionStorage(FileInfo storageFile)
        {
            _storageFile = storageFile;
        }

        public void Store(object version)
        {
            using (FileStream stream = !_storageFile.Exists ? _storageFile.Create() : _storageFile.OpenWrite())
            {
                var bytes = Encoding.Default.GetBytes(version.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public object Load()
        {
            using (FileStream stream = !_storageFile.Exists ? _storageFile.Create() : _storageFile.OpenRead())
            {
                var bytes = Encoding.Default.GetBytes(version.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }


        }
    }
}