using System;

namespace FileSpliter.Models
{
    [Serializable]
    public class FilePart
    {
        public FilePartInfo PartInfo { get; set; }
        public FileSummaryInfo SummaryInfo { get; set; }
        public byte[] DataBytesArray { get; set; }
        public bool IsAvailable { get; set; }
    }
}