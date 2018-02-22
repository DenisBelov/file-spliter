using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL
{
    class StreamProvider : IStreamProvider
    {
        public File SplitStream(Stream stream, int partsCount, string fileName)
        {
            var file = new File();
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytesArray = memoryStream.ToArray();
            file.FileParts = new List<FilePart>();
            if (partsCount < bytesArray.Length)
            {
                for (int i = 0; i < partsCount; i++)
                {
                    var filePart = new FilePart
                    {
                        Id = new Guid(),
                        Name = fileName + i,
                        PartNumber = i,
                        SummaryInfo = new FileSummaryInfo
                        {
                            FileName = fileName,
                            FileParts = new List<FilePartInfo>()
                        }
                    };
                    var arraySize = i == partsCount - 1
                        ? bytesArray.Length / partsCount
                        : (int)Math.Round((double)bytesArray.Length / partsCount);
                    var bytesArrayPart = new byte[arraySize];
                    for (int j = 0; j < arraySize; j++)
                    {
                        bytesArrayPart[j] = bytesArray[arraySize * i + j];
                    }
                    filePart.DataBytesArray = bytesArrayPart;
                    file.FileParts.Add(filePart);
                }
            }
            foreach (var filePart in file.FileParts)
            {
                filePart.SummaryInfo.FileParts = new List<FilePartInfo>(file.FileParts.Select(f => f as FilePartInfo));
            }
            return file;
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
