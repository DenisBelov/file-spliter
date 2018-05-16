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
                        Id = filePart.Id,
                        IsAvailable = true,
                        Name = filePart.Name,
                        FileName = filePart.SummaryInfo.FileName,
                        Size = filePart.DataBytesArray.Length
                    });
                }

                OnPropertyChanged(nameof(FilePart));
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
    }
}
