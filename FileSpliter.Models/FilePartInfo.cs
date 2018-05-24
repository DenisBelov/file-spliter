using System;

namespace FileSpliter.Models
{
    [Serializable]
    public class FilePartInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int PartNumber { get; set; }
        public long PartSize { get; set; }
    }
}
