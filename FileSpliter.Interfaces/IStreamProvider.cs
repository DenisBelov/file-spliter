using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.Interfaces
{
    public interface IStreamProvider
    {
        Task<File> SplitFileAsync(string path, int partsCount, string fileName = null);
        FileStream MergeStreams(IEnumerable<FilePart> parts, string fileName);
    }
}