using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileSpliter.Models;

namespace FileSpliter.Interfaces
{
    public interface IFileSerializator
    {
        Task WriteFilePart(FilePart filePart, string fileName, string folderPath);
        FilePart ReadFilePart(string path);
        Task<List<FilePart>> ReadAllFilePartsFromFolder(string path, string fileId, string fileName = null);
    }
}
