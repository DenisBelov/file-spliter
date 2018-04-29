using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.Interfaces
{
    public interface IFileSerializator
    {
        Task WriteFilePart(FilePart filePart, string folderPath);
        Task<FilePart> ReadFilePart(string path);
    }
}
