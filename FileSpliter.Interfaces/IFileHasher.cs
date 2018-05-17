using System.IO;

namespace FileSpliter.Interfaces
{
    public interface IFileHasher
    {
        string Hash(string path, FileStream file = null);
    }
}