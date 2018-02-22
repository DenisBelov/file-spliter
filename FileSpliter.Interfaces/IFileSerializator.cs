using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.Interfaces
{
    public interface IFileSerializator
    {
        Task WriteFile(File file, string folderPath);
        Task<FilePart> ReadFilePart(string path);
    }
}
