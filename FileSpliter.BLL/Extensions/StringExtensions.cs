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
        public static string GetSize(this string value)
        {
            if (!long.TryParse(value, out var size))
            {
                return string.Empty;
            }

            var str = string.Empty;

            if (size >= 1024L)
            {
                size /= 1024L;
                str = " KB";

                if (size < 1024L) return size + str;

                size /= 1024L;
                str = " MB";

                if (size < 1024L) return size + str;

                size /= 1024L;
                str = " GB";
            }
            else
            {
                return size + " B";
            }
            return size + str;
        }
    }
}
