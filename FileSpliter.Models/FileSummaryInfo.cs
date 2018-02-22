using System;
using System.Collections.Generic;

namespace FileSpliter.Models
{
    [Serializable]
    public class FileSummaryInfo
    {
        public string FileName { get; set; }
        public List<FilePartInfo> FileParts { get; set; }
    }
}
