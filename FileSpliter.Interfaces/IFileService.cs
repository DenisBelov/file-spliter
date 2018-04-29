using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.Interfaces
{
    public interface IFileService
    {
        File Split(string path, int partsCount);
        void SaveParts(File file, string path);
        void SaveFile(File file, string path);
        Task<File> ReadFilePart(string path);
    }
}