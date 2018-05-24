using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.BLL
{
    public interface IMemoryBufferManager
    {
        Task Flush();
        FilePartBuffered GetFilePart(string id);
        FilePartBuffered GetFilePartByName(string name);
        Task SaveFilePart(FilePart filePart);
    }
}