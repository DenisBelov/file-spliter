using System;
using System.IO;

namespace FileSpliter.BLL.Extensions
{
    public static class StringExtensions
    {
        public static string GetDirectoryName(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            if (Directory.Exists(fileName))
            {
                return fileName;
            }
            var slashIndex = fileName.LastIndexOf("\\", StringComparison.Ordinal);
            if (slashIndex != -1)
            {
                fileName = fileName.Substring(0, slashIndex);
            }
            return Directory.Exists(fileName) ? fileName : null;
        }
    }
}
