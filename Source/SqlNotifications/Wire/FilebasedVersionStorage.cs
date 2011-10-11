using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LandauMedia.Wire
{
    public class FilebasedVersionStorage : IVersionStorage
    {
        readonly FileInfo _storageFile;

        public FilebasedVersionStorage(string storageFile)
            : this(new FileInfo(storageFile))
        {
        }

        public FilebasedVersionStorage(FileInfo storageFile)
        {
            if(storageFile == null)
                throw new ArgumentNullException("storageFile");

            if (storageFile.Directory == null)
                throw new ArgumentNullException("storageFile");

            _storageFile = storageFile;

            if (!_storageFile.Directory.Exists)
                _storageFile.Directory.Create();

            using (_storageFile.Create())
            {
            }
        }

        public void Store(string key, ulong version)
        {
            var keyvaluePairs = Read();

            if (keyvaluePairs.ContainsKey(key))
            {
                keyvaluePairs[key] = version;
            }
            else
            {
                keyvaluePairs.Add(key, version);
            }

            Write(keyvaluePairs);

        }

        public ulong Load(string key)
        {
            var keyValuePairs = Read();
            return keyValuePairs.ContainsKey(key) ? keyValuePairs[key] : 0;
        }

        private void Write(IDictionary<string, ulong> values)
        {
            using (StreamWriter writer = new StreamWriter(_storageFile.FullName, false))
            {
                foreach(var pair in values)
                {
                    writer.Write(pair.Key + "=" + pair.Value);
                }
            }
        }

        private IDictionary<string, ulong> Read()
        {
            using (StreamReader reader = new StreamReader(_storageFile.FullName, Encoding.Default))
            {
                return reader.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t =>
                    {
                        var strings = t.Split('=');
                        return new {Key = strings[0], value = ulong.Parse(strings[1])};
                    })
                    .ToDictionary(t => t.Key, t => t.value);
            }
        }
    }
}