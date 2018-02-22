using System;

namespace FileSpliter.Models
{
    [Serializable]
    public class FilePartInfo
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int PartNumber { get; set; }
    }
}
