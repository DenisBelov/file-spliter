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
        private readonly IFileSerializator _fileSerializator;
        private readonly IStreamProvider _streamProvider;
        private readonly IFileHasher _fileHasher;

        public FileService(IFileSerializator serializator, IStreamProvider streamProvider, IFileHasher fileHasher)
        {
            _fileSerializator = serializator;
            _streamProvider = streamProvider;
            _fileHasher = fileHasher;
        }

        public async Task<File> Split(string path, int partsCount)
        {
            var file = _streamProvider.SplitFileAsync(path, partsCount);
            return await file;
        }

        public async Task SaveParts(File file, string path)
        {
            if (file != null)
            {
                foreach (var filePart in file.FileParts)
                {
                    await _fileSerializator.WriteFilePart(filePart, filePart.PartInfo.Name, path);
                }
            }
        }

        public void SaveFile(File file, string path)
        {
            _streamProvider.MergeStreams(file.FileParts, path).Close();
        }

        public File ReadFilePart(string path)
        {
            var part = _fileSerializator.ReadFilePart(path);
            var file = new File(part.SummaryInfo.FileId)
            {
                Id = part.SummaryInfo.FileId
            };
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

        public async Task<File> ReadAllFileParts(string path, File file)
        {
            var fileParts = await _fileSerializator.ReadAllFilePartsFromFolder(path, file.Id);
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
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return stream.Length > 100 ? 100 : (int) stream.Length;
            }
        }

        public long GetPossiblePartsLength(string path, int count)
        {
            if (count != 0)
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    return stream.Length / count;
                }
            }
            return 0;
        }
    }
}
