using System.Collections.Generic;
using System.IO;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.Interfaces
{
    public interface IStreamProvider
    {
        File SplitFile(string path, int partsCount, string fileName = null);
        Stream MergeStreams(IEnumerable<FilePart> parts);
    }
}