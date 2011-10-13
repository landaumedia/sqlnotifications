using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LandauMedia.Wire;

namespace LandauMedia.Storage
{
    public class FilebasedVersionStorage : IVersionStorage
    {
        static object _lock = new object();

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
            if (key.Contains("="))
                throw new ArgumentException("key must not contain =");

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
            if (key.Contains("="))
                throw new ArgumentException("key must not contain =");

            var keyValuePairs = Read();
            return keyValuePairs.ContainsKey(key) ? keyValuePairs[key] : 0;
        }

        private void Write(IDictionary<string, ulong> values)
        {
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(_storageFile.FullName, false, Encoding.Default))
                {
                    foreach (var pair in values)
                    {
                        writer.WriteLine(pair.Key + "=" + pair.Value);
                    }
                }
            }
        }

        private IDictionary<string, ulong> Read()
        {
            lock(_lock)
            {
                using (StreamReader reader = new StreamReader(_storageFile.FullName, Encoding.Default))
                {
                    return reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t =>
                        {
                            var strings = t.Split('=');
                            return new { Key = strings[0], value = ulong.Parse(strings[1]) };
                        })
                        .ToDictionary(t => t.Key, t => t.value);
                }    
            }
        }
    }
}