using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL.Services
{
    public class FileService : IFileService
    {
        private IFileSerializator _fileSerializator;
        private IStreamProvider _streamProvider;
        private IFileHasher _fileHasher;

        public FileService(IFileSerializator serializator, IStreamProvider streamProvider, IFileHasher fileHasher)
        {
            _fileSerializator = serializator;
            _streamProvider = streamProvider;
            _fileHasher = fileHasher;
        }

        public File Split(string path, int partsCount)
        {
            var file = _streamProvider.SplitFile(path, partsCount);
            return file;

        }

        public void SaveParts(File file, string path)
        {
            foreach (var filePart in file.FileParts)
            {
                _fileSerializator.WriteFilePart(filePart, path);
            }
        }

        public void SaveFile(File file, string path)
        {
            using (var streamToWrite = _streamProvider.MergeStreams(file.FileParts))
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    streamToWrite.CopyTo(stream);
                }
            }
        }

        public async Task<File> ReadFilePart(string path)
        {
            var part = await _fileSerializator.ReadFilePart(path);
            var file = new File(part.SummaryInfo.FileId);
            file.Id = part.SummaryInfo.FileId;
            file.FileParts.Add(part);
            foreach (var filePart in part.SummaryInfo.FileParts.Where(f => f.Id != part.PartInfo.Id))
            {
                file.FileParts.Add(new FilePart
                {
                    PartInfo = new FilePartInfo
                    {
                        Id = filePart.Id,
                        Name = filePart.Name,
                        PartNumber = filePart.PartNumber
                    }
                });
            }

            return file;
        }

        public File ReadAllFileParts(string path, File file)
        {
            var fileParts = _fileSerializator.ReadAllFilePartsFromFolder(path, file.Id).Result;
            file.FileParts = file.FileParts
                .Where(f => f.IsAvailable && !fileParts.Exists(p => p.PartInfo.Id == f.PartInfo.Id))
                .Union(fileParts).ToList();
            file.FileParts = file.FileParts.Union(file.FileParts.FirstOrDefault(p => p.SummaryInfo != null)?.SummaryInfo.FileParts
                    .Where(f => !file.FileParts.Exists(p => p.PartInfo.Id == f.Id))
                    .Select(f => new FilePart {PartInfo = f})?? new List<FilePart>()).ToList();
            return file;
        }

        public int GetPossiblePartsCount(string path)
        {
            return System.IO.File.ReadAllBytes(path).Length;
        }
    }
}
