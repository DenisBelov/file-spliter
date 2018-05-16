using System;
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
                var file = new File();
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                var bytesArray = memoryStream.ToArray();
                file.FileParts = new List<FilePart>();
                var fileId = Guid.NewGuid();
                if (partsCount < bytesArray.Length)
                {
                    for (int i = 0, calculatedSize = 0; i < partsCount; i++)
                    {
                        var filePart = new FilePart
                        {
                            Id = fileId,
                            Name = fileName + "_part" + (i + 1),
                            PartNumber = i + 1,
                            SummaryInfo = new FileSummaryInfo
                            {
                                FileName = fileName,
                                FileParts = new List<FilePartInfo>()
                            }
                        };
                        var arraySize = /*i == partsCount - 1 ?*/
                            (bytesArray.Length - calculatedSize) / (partsCount - i);
                            //: (int)Math.Floor((double)bytesArray.Length / partsCount);
                        var bytesArrayPart = new byte[arraySize];
                        for (int j = 0; j < arraySize; j++)
                        {
                            bytesArrayPart[j] = bytesArray[calculatedSize + j];
                        }
                        filePart.DataBytesArray = bytesArrayPart;
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
                    filePart.SummaryInfo.FileParts = new List<FilePartInfo>(file.FileParts.Select(f => f as FilePartInfo));
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
