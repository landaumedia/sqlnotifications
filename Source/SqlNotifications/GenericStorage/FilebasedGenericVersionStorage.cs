using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;

namespace LandauMedia.GenericStorage
{
    public class FilebasedGenericVersionStorage : IGenericVersionStorage
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static object _lock = new object();

        readonly FileInfo _storageFile;

        public FilebasedGenericVersionStorage(string storageFile)
            : this(new FileInfo(storageFile))
        {
            Verbose = false;
        }

        public bool Verbose { get;  set; }

        public FilebasedGenericVersionStorage(FileInfo storageFile)
        {
            if(storageFile == null)
                throw new ArgumentNullException("storageFile");

            if (storageFile.Directory == null)
                throw new ArgumentNullException("storageFile");

            _storageFile = storageFile;

            if (!_storageFile.Directory.Exists)
                _storageFile.Directory.Create();

            // create if not exist
            if (!storageFile.Exists)
            {
                using (_storageFile.Create()){}    
            }
        }

        public void Store(string key, string version)
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

        public string Load(string key)
        {
            if (key.Contains("="))
                throw new ArgumentException("key must not contain =");

            var pairs = Read();

            if (Verbose)
                Logger.Debug(string.Format("Load Key to VersionStorage: key:{0}", key));

            return pairs.ContainsKey(key) ? pairs[key] : null;
        }

        public bool Exist(string key)
        {
            return Read().Any(g => g.Key == key);
        }

        public void Reset()
        {
            if (_storageFile.Exists)
                _storageFile.Delete();
        }

        void Write(IEnumerable<KeyValuePair<string, string>> values)
        {
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(_storageFile.FullName, false, Encoding.Default))
                {
                    foreach (var pair in values)
                    {
                        if (Verbose)
                            Logger.Debug(string.Format("Store Key to VersionStorage: key:{0} value:{1}", pair.Key, pair.Value));

                        writer.WriteLine(pair.Key + "=" + pair.Value);
                    }
                }
            }
        }

        IDictionary<string, string> Read()
        {
            lock(_lock)
            {
                using (StreamReader reader = new StreamReader(_storageFile.FullName, Encoding.Default))
                {
                    return reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t =>
                        {
                            var strings = t.Split('=');
                            return new { Key = strings[0], value = strings[1] };
                        })
                        .ToDictionary(t => t.Key, t => t.value);
                }    
            }
        }
    }
}