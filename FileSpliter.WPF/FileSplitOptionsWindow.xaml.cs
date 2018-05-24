using System;
using System.Windows;
using FileSpliter.BLL.Extensions;
using FileSpliter.Interfaces;
using Microsoft.Win32;

namespace FileSpliter.WPF
{
    /// <summary>
    /// Interaction logic for FileSplitOptionsWindow.xaml
    /// </summary>
    public partial class FileSplitOptionsWindow
    {
        private readonly Action<int, string> _callback;
        private string _fileName;
        private readonly IFileService _fileService;
        public FileSplitOptionsWindow(Action<int, string> callback, IFileService fileService)
        {
            InitializeComponent();
            _callback = callback;
            _fileService = fileService;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _callback((int)Slider.Value, _fileName);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
            {
                Slider.Maximum = Math.Min(100, _fileService.GetPossiblePartsCount(openFileDialog.FileName));
                _fileName = openFileDialog.FileName;
            }
            else
            {
                Close();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FileLengthLabel.Content = _fileService.GetPossiblePartsLength(_fileName, (int) Slider.Value).ToString().GetSize();
        }
    }
}
