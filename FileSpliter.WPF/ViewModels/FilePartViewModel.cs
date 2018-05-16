using System;

namespace FileSpliter.WPF.ViewModels
{
    public class FilePartViewModel
    {
        public int Size { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
