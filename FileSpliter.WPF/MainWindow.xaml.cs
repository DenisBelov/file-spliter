using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using FileSpliter.BLL;
using FileSpliter.BLL.Services;
using FileSpliter.Interfaces;
using FileSpliter.Models;
using FileSpliter.WPF.Exceptions;
using FileSpliter.WPF.Properties;
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
        private readonly IFileService _fileService;
        private readonly MainWindowViewModel _viewModel;
        private readonly MemoryBufferManager _memoryBufferManager;
        private const string BufferPath = "BufferPath";
        private const string BufferName = "FileSpliterBuffer";

        public MainWindow()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default[BufferPath].ToString()))
            {
                Settings.Default[BufferPath] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + BufferName;
                Settings.Default.Save();
            }
            _viewModel = new MainWindowViewModel();
            InitializeComponent();
            DataContext = _viewModel;
            _memoryBufferManager = new MemoryBufferManager(Settings.Default[BufferPath].ToString());
            var serializator = new FileSerializator(_memoryBufferManager);
            _fileService = new FileService(serializator,
                new StreamProvider(new FileHasher(), serializator, _memoryBufferManager), new FileHasher());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _memoryBufferManager.Flush().Wait();
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

        private async void SplitFile(int partsCount, string filePath)
        {
            try
            {
                LoaderGrid.Visibility = Visibility.Visible;
                var file = _fileService.Split(filePath, partsCount);
                _viewModel.File = await file;
                LoaderGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                LoaderGrid.Visibility = Visibility.Collapsed;
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
                        LoaderGrid.Visibility = Visibility.Visible;
                        var file = _fileService.ReadFilePart(openFileDialog.FileName);
                        file = await _fileService.ReadAllFileParts(openFileDialog.FileName, file);
                        _viewModel.File = file;
                        LoaderGrid.Visibility = Visibility.Collapsed;
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
            finally
            {
                LoaderGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void DockPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindowForm.DragMove();
        }

        private async void SavePartsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoaderGrid.Visibility = Visibility.Visible;
                    await _fileService.SaveParts(_viewModel.File, folderBrowserDialog.SelectedPath);
                    LoaderGrid.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                LoaderGrid.Visibility = Visibility.Collapsed;
            }
        }

        private async void OpenPart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog().Value)
                {
                    try
                    {
                        LoaderGrid.Visibility = Visibility.Visible;
                        var file = await _fileService.ReadAllFileParts(openFileDialog.FileName, _viewModel.File);
                        _viewModel.File = file;
                        LoaderGrid.Visibility = Visibility.Collapsed;
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
            finally
            {
                LoaderGrid.Visibility = Visibility.Collapsed;
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