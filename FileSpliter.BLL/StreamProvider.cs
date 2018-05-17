﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL
{
    public class StreamProvider : IStreamProvider
    {
        private IFileHasher _fileHasher;

        public StreamProvider(IFileHasher fileHasher)
        {
            _fileHasher = fileHasher;
        }

        public File SplitFile(string path, int partsCount, string fileName = null)
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
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                var bytesArray = memoryStream.ToArray();
                file.FileParts = new List<FilePart>();
                if (partsCount <= bytesArray.Length)
                {
                    for (int i = 0, calculatedSize = 0; i < partsCount; i++)
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
                        var arraySize =
                            (bytesArray.Length - calculatedSize) / (partsCount - i);
                        var bytesArrayPart = new byte[arraySize];
                        for (int j = 0; j < arraySize; j++)
                        {
                            bytesArrayPart[j] = bytesArray[calculatedSize + j];
                        }
                        filePart.DataBytesArray = bytesArrayPart;
                        filePart.PartInfo.Id += bytesArrayPart.Length;
                        file.FileParts.Add(filePart);
                        calculatedSize += arraySize;
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

        public Stream MergeStreams(IEnumerable<FilePart> parts)
        {
            var fileParts = parts as FilePart[] ?? parts.ToArray();
            var sumBuffer = new byte[fileParts.Sum(p => p.DataBytesArray.Length)];
            for (int i = 0; i < sumBuffer.Length;)
            {
                foreach (var filePart in fileParts)
                {
                    for (int j = 0; j < filePart.DataBytesArray.Length; j++, i++)
                    {
                        sumBuffer[i] = filePart.DataBytesArray[j];
                    }
                }
            }
            return new MemoryStream(sumBuffer);
        }
    }
}
