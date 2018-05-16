using System;
using System.Windows;
using Microsoft.Win32;

namespace FileSpliter.WPF
{
    /// <summary>
    /// Interaction logic for FileSplitOptionsWindow.xaml
    /// </summary>
    public partial class FileSplitOptionsWindow
    {
        private readonly Action<int, string> _callback;
        private readonly string _fileName;
        public FileSplitOptionsWindow(Action<int, string> callback)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().Value)
            {
                _callback = callback;
                InitializeComponent();
                _fileName = openFileDialog.FileName;
            }
            else
            {
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _callback((int)slider.Value, _fileName);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
