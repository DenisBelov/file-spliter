using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileSpliter.BLL.Extensions;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using Newtonsoft.Json;

namespace FileSpliter.BLL
{
    public class FileSerializator : IFileSerializator
    {
        private readonly MemoryBufferManager _memoryBufferManager;

        public FileSerializator(MemoryBufferManager memoryBufferManager)
        {
            _memoryBufferManager = memoryBufferManager;
        }

        public Task WriteFilePart(FilePart filePart, string fileName, string folderPath)
        {
            return Task.Run(() =>
            {
                using (var fileStream =
                    new FileStream(folderPath + "\\" + fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    FilePartBuffered bufferPart = null;
                    if (filePart.DataBytesArray == null)
                    {
                        bufferPart = _memoryBufferManager.GetFilePartByName(fileName);
                        filePart.DataBytesArray = bufferPart?.Data;
                    }
                    using (var writer = new StreamWriter(fileStream))
                    {
                        JsonSerializer serialiser = new JsonSerializer();
                        serialiser.Serialize(writer, filePart);
                    }
                    if (bufferPart != null)
                    {
                        filePart.DataBytesArray = null;
                        GC.Collect();
                    }
                }
            });
        }

        public FilePart ReadFilePart(string path)
        {

                try
                {
                    using (var streamReader = new StreamReader(path))
                    {
                        using (var reader = new JsonTextReader(streamReader))
                        {
                            JsonSerializer serialiser = new JsonSerializer();
                            return serialiser.Deserialize<FilePart>(reader);
                        }
                    }
                }
                catch (JsonReaderException)
                {
                }
                catch (JsonSerializationException)
                {
                }
                return null;

        }

        public Task<List<FilePart>> ReadAllFilePartsFromFolder(string path, string fileId, string fileName = null)
        {
            return Task.Run(async () =>
            {
                var result = new List<FilePart>();
                var directory = new DirectoryInfo(path.GetDirectoryName());
                var files = directory.GetFiles(fileName ?? "*");
                foreach (var file in files)
                {
                    try
                    {
                        var filePart = ReadFilePart(file.FullName);
                        if (filePart != null && filePart.SummaryInfo.FileId == fileId)
                        {
                            result.Add(filePart);
                        }
                    }
                    catch (OutOfMemoryException)
                    {
                        foreach (var filePart in result)
                        {
                            await _memoryBufferManager.SaveFilePart(filePart);
                        }
                    }
                }
                return result;
            });
        }
    }
}