using System;
using FileSpliter.Models;

namespace FileSpliter.WPF.ViewModels
{
    public class FilePartViewModel
    {
        public FilePartViewModel()
        {
            
        }

        public FilePartViewModel(FilePart filePart)
        {
            Size = filePart.PartInfo.PartSize;
            Name = filePart.PartInfo.Name;
            Number = filePart.PartInfo.PartNumber;
            IsAvailable = true;
            FileName = filePart.SummaryInfo.FileName;
        }

        public FilePartViewModel(FilePartInfo filePartInfo)
        {
            Name = filePartInfo.Name;
            Number = filePartInfo.PartNumber;
            IsAvailable = false;
            Id = filePartInfo.Id;
        }

        public int Number { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsAvailable { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
