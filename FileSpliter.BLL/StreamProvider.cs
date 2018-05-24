using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using Microsoft.Win32.SafeHandles;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL
{
    public class StreamProvider : IStreamProvider
    {
        private readonly IFileHasher _fileHasher;
        private readonly IFileSerializator _fileSerializator;
        private readonly IMemoryBufferManager _bufferManager;

        public StreamProvider(IFileHasher fileHasher, IFileSerializator fileSerializator, IMemoryBufferManager bufferManager)
        {
            _fileHasher = fileHasher;
            _fileSerializator = fileSerializator;
            _bufferManager = bufferManager;
        }

        public async Task<File> SplitFileAsync(string path, int partsCount, string fileName = null)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    var slashIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
                    var name = slashIndex > 0 ? path.Substring(slashIndex + 1, path.Length - slashIndex - 1) : path;
                    var dotIndex = name.LastIndexOf(".", StringComparison.Ordinal);
                    fileName = dotIndex > 0 ? name.Substring(0, dotIndex) : name;
                }
                var file = new File(_fileHasher.Hash(path, stream));
                var fileSize = stream.Length;

                file.FileParts = new List<FilePart>();
                if (partsCount <= fileSize)
                {
                    long calculatedSize = 0;
                    for (int i = 0; i < partsCount; i++)
                    {
                        var filePart = new FilePart
                        {
                            PartInfo = new FilePartInfo
                            {
                                Id = file.Id + i,
                                Name = fileName + "_part" + (i + 1),
                                PartNumber = i + 1
                            },
                            SummaryInfo = new FileSummaryInfo
                            {
                                FileId = file.Id,
                                FileName = fileName,
                                FileParts = new List<FilePartInfo>()
                            },
                            IsAvailable = true
                        };
                        try
                        {
                            var arraySize =
                                (fileSize - calculatedSize) / (partsCount - i);
                            filePart.PartInfo.PartSize = arraySize;
                            var bytesArrayPart = new byte[arraySize];
                            stream.Read(bytesArrayPart, 0, (int)arraySize);
                            filePart.DataBytesArray = bytesArrayPart;
                            filePart.PartInfo.Id += bytesArrayPart.Length;
                            file.FileParts.Add(filePart);
                            calculatedSize += arraySize;
                        }
                        catch (OutOfMemoryException)
                        {
                            foreach (var part in file.FileParts)
                            {
                                await _bufferManager.SaveFilePart(part);
                            }
                            GC.Collect();
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Stream length is less than parts count");
                }
                foreach (var filePart in file.FileParts)
                {
                    filePart.SummaryInfo.FileParts = new List<FilePartInfo>(file.FileParts.Select(f => f.PartInfo));
                }
                return file;
            }
        }

        public FileStream MergeStreams(IEnumerable<FilePart> parts, string fileName)
        {
            var fileParts = parts as FilePart[] ?? parts.ToArray();
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);

            foreach (var filePart in fileParts)
            {
                fileStream.Write(filePart.DataBytesArray, 0, (int)filePart.PartInfo.PartSize);
            }
            return fileStream;
        }
    }
}