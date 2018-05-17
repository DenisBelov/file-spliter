using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSpliter.Models
{
    public class File
    {
        public File(string id)
        {
            Id = id;
            FileParts = new List<FilePart>();
        }
        public string Id { get; set; }
        private List<FilePart> _fileParts;

        public List<FilePart> FileParts
        {
            get { return _fileParts = _fileParts.OrderBy(f => f.PartInfo.PartNumber).ToList(); }
            set => _fileParts = value;
        }

    }
}
