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
        public Task WriteFilePart(FilePart filePart, string folderPath)
        {
            return Task.Run(() =>
            {
                using (var fileStream =
                    new FileStream(folderPath + "\\" + filePart.SummaryInfo.FileName + "_part" + filePart.PartInfo.PartNumber,
                        FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var writer =
                        new StreamWriter(fileStream))
                    {
                        JsonSerializer serialiser = new JsonSerializer();
                        serialiser.Serialize(writer, filePart);
                    }
                }
            });
        }

        public Task<FilePart> ReadFilePart(string path)
        {
            return Task.Run(() =>
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
            });
        }

        public Task<List<FilePart>> ReadAllFilePartsFromFolder(string path, string id, string fileName = null)
        {
            return Task.Run(() =>
            {
                var result = new List<FilePart>();
                var directory = new DirectoryInfo(path.GetDirectoryName());
                var files = directory.GetFiles(fileName ?? "*");
                foreach (var file in files)
                {
                    var filePart = ReadFilePart(file.FullName).Result;
                    if (filePart != null && filePart.SummaryInfo.FileId == id)
                    {
                        result.Add(filePart);
                    }
                }
                return result;
            });
        }
    }
}
