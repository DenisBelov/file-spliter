using System.IO;
using System.Threading.Tasks;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using Newtonsoft.Json;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL
{
    public class FileSerializator : IFileSerializator
    {
        public Task WriteFilePart(FilePart filePart, string folderPath)
        {
            return Task.Run(() =>
            {
                using (var fileStream =
                    new FileStream(folderPath + filePart.SummaryInfo.FileName + filePart.PartNumber,
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
                using (var streamReader = new StreamReader(path))
                {
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        JsonSerializer serialiser = new JsonSerializer();
                        return serialiser.Deserialize<FilePart>(reader);
                    }
                }
            });
        }
    }
}
