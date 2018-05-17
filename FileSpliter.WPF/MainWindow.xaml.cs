using System;
using System.Windows;
using System.Windows.Forms;
using FileSpliter.BLL;
using FileSpliter.BLL.Services;
using FileSpliter.Interfaces;
using FileSpliter.WPF.Exceptions;
using FileSpliter.WPF.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

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
            _fileService = new FileService(new FileSerializator(), new StreamProvider(new FileHasher()), new FileHasher());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePartsWindow = new FileSplitOptionsWindow(SplitFile, _fileService);
                filePartsWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SplitFile(int partsCount, string filePath)
        {
            try
            {
                var file = _fileService.Split(filePath, partsCount);
                _viewModel.File = file;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OpenPartsButtoon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog().Value)
                {
                    try
                    {
                        var file = await _fileService.ReadFilePart(openFileDialog.FileName);
                        file = _fileService.ReadAllFileParts(openFileDialog.FileName, file);
                        _viewModel.File = file;
                    }
                    catch (InvalidFormatException exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DockPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindowForm.DragMove();
        }

        private void SavePartsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _fileService.SaveParts(_viewModel.File, folderBrowserDialog.SelectedPath);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenPart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog().Value)
                {
                    try
                    {
                        var file = _fileService.ReadAllFileParts(openFileDialog.FileName, _viewModel.File);
                        _viewModel.File = file;
                    }
                    catch (InvalidFormatException exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog().Value)
                {
                    _fileService.SaveFile(_viewModel.File, saveFileDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Minimaze_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
