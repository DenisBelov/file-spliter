using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.Interfaces
{
    public interface IFileService
    {
        Task<File> Split(string path, int partsCount);
        Task SaveParts(File file, string path);
        void SaveFile(File file, string path);
        File ReadFilePart(string path);
        Task<File> ReadAllFileParts(string path, File file);
        int GetPossiblePartsCount(string path);
        long GetPossiblePartsLength(string path, int count);
    }
}