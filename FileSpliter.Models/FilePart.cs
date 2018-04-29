using System;

namespace FileSpliter.Models
{
    [Serializable]
    public class FilePart : FilePartInfo
    {
        public FileSummaryInfo SummaryInfo { get; set; }
        public byte[] DataBytesArray { get; set; }
    }
}