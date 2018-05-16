using System.Windows;
using FileSpliter.BLL;
using FileSpliter.BLL.Services;
using FileSpliter.Interfaces;
using FileSpliter.WPF.ViewModels;
using Microsoft.Win32;

namespace FileSpliter.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IFileService _fileService;
        private readonly MainWindowViewModel _viewModel;
        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            InitializeComponent();
            DataContext = _viewModel;
            _fileService = new FileService(new FileSerializator(), new StreamProvider());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var filePartsWindow = new FileSplitOptionsWindow(SplitFile);
            filePartsWindow.ShowDialog();
            
        }

        private void SplitFile(int partsCount, string filePath)
        {
            var file = _fileService.Split(filePath, partsCount);
            _viewModel.File = file;
        }

        private void OpenPartsButtoon_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DockPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindowForm.DragMove();
        }

    }
}
