using System;
using System.IO;
using System.Threading.Tasks;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using Newtonsoft.Json;

namespace FileSpliter.BLL
{
    public class MemoryBufferManager : IMemoryBufferManager
    {
        private readonly string _bufferPath;


        public MemoryBufferManager(string bufferPath)
        {
            _bufferPath = bufferPath;
            if (!Directory.Exists(bufferPath))
            {
                Directory.CreateDirectory(bufferPath);
            }
        }

        public FilePartBuffered GetFilePart(string id)
        {
            var directory = new DirectoryInfo(_bufferPath);
            var files = directory.GetFiles("*");
            foreach (var file in files)
            {
                using (var reader = new StreamReader(file.FullName))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        JsonSerializer serialiser = new JsonSerializer();
                        var filePart = serialiser.Deserialize<FilePartBuffered>(jsonReader);
                        if (filePart != null && filePart.Id == id)
                        {
                            return filePart;
                        }
                    }
                }
            }
            return null;
        }

        public FilePartBuffered GetFilePartByName(string name)
        {
            var file = System.IO.File.OpenRead(_bufferPath + "\\" + name);
            using (var reader = new StreamReader(file))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serialiser = new JsonSerializer();
                    var filePart = serialiser.Deserialize<FilePartBuffered>(jsonReader);
                    return filePart;
                }
            }
        }

        public Task SaveFilePart(FilePart filePart)
        {
            return Task.Run(() =>
            {
                using (var fileStream = new FileStream(_bufferPath + "\\" + filePart.PartInfo.Name, FileMode.OpenOrCreate))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        JsonSerializer serialiser = new JsonSerializer();
                        serialiser.Serialize(writer, new FilePartBuffered { Data = filePart.DataBytesArray, Id = filePart.PartInfo.Id });
                    }
                }
                filePart.DataBytesArray = null;
                GC.Collect();
            });
        }

        public Task Flush()
        {
            return Task.Run(() =>
            {
                var directory = new DirectoryInfo(_bufferPath);
                var files = directory.GetFiles("*");
                foreach (var file in files)
                {
                    System.IO.File.Delete(file.FullName);
                }
            });
        }
    }
}