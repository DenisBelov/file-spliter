using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FileSpliter.Models;
using FileSpliter.WPF.Annotations;

namespace FileSpliter.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private File _file;
        public File File
        {
            get => _file;
            set
            {
                _file = value;
                FileParts.Clear();
                foreach (var filePart in _file.FileParts)
                {
                    FileParts.Add(new FilePartViewModel
                    {
                        Id = filePart.PartInfo.Id,
                        IsAvailable = filePart.IsAvailable,
                        Name = filePart.PartInfo.Name,
                        FileName = filePart.SummaryInfo?.FileName,
                        Size = filePart.DataBytesArray?.Length ?? 0
                    });
                }

                OnPropertyChanged(nameof(FileParts));
            }
        }

        public ObservableCollection<FilePartViewModel> FileParts { get; set; }

        public MainWindowViewModel()
        {
            FileParts = new ObservableCollection<FilePartViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<FilePartInfo> GetFilePartsInfo(IEnumerable<List<FilePartInfo>> parts)
        {
            var result = new List<FilePartInfo>();
            foreach (var part in parts)
            {
                result = result.Where(p => !part.Exists(f => f.Id == p.Id)).Union(part).ToList();
            }
            return result;
        }
    }
}
