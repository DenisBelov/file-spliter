using System.Collections.Generic;
using System.IO;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.Interfaces
{
    public interface IStreamProvider
    {
        File SplitStream(Stream stream, int partsCount, string fileName);
        Stream MergeStreams(IEnumerable<FilePart> parts);
    }
}