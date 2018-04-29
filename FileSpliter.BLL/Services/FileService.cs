﻿using System;
using System.IO;
using System.Threading.Tasks;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using File = FileSpliter.Models.File;

namespace FileSpliter.BLL.Services
{
    public class FileService : IFileService
    {
        public IFileSerializator FileSerializator { get; set; }

        public IStreamProvider StreamProvider { get; set; }

        public FileService()
        {
            FileSerializator = new FileSerializator();
            StreamProvider = new StreamProvider();
        }

        public File Split(string path, int partsCount)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var slashIndex = path.IndexOf("/", StringComparison.Ordinal);
                var name = slashIndex > 0 ? path.Substring(slashIndex, path.Length - slashIndex) : path;
                var dotIndex = path.LastIndexOf(".", StringComparison.Ordinal);
                name = dotIndex > 0 ? name.Substring(0, dotIndex) : name;
                var file = StreamProvider.SplitStream(stream, partsCount, name);
                return file;
            }
        }

        public void SaveParts(File file, string path)
        {
            foreach (var filePart in file.FileParts)
            {
                FileSerializator.WriteFilePart(filePart, path);
            }
        }

        public void SaveFile(File file, string path)
        {
            using (var streamToWrite = StreamProvider.MergeStreams(file.FileParts))
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    streamToWrite.CopyTo(stream);
                }
            }
        }

        public async Task<File> ReadFilePart(string path)
        {
            var part = await FileSerializator.ReadFilePart(path);
            var file = new File();
            file.FileParts.Add(part);
            foreach (var filePart in part.SummaryInfo.FileParts)
            {   
                file.FileParts.Add(new FilePart
                {
                    IsAvailable = false,
                    Id = filePart.Id,
                    Name = filePart.Name,
                    PartNumber = filePart.PartNumber
                });
            }

            return file;
        }
    }
}
