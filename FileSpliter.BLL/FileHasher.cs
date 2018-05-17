using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FileSpliter.Interfaces;

namespace FileSpliter.BLL
{
    public class FileHasher : IFileHasher
    {
        public string Hash(string path, FileStream file)
        {
            try
            {
                if (File.Exists(path))
                {
                    byte[] result;
                    byte[] fileValue = new byte[2048];
                    file?.Read(fileValue, 0, 2048);
                    var fileLenght = file?.Length ?? 0;
                    var creationTime = File.GetCreationTimeUtc(path);

                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                        {
                            writer.Write(creationTime.Ticks);
                            writer.Write(fileLenght);
                            writer.Write(fileValue);
                        }

                        stream.Position = 0;

                        using (var hash = SHA256.Create())
                        {
                            result = hash.ComputeHash(stream);
                        }
                    }

                    var text = new string[20];

                    for (var i = 0; i < text.Length; i++)
                    {
                        text[i] = result[i].ToString("x2");
                    }

                    return string.Concat(text);
                }
                return Guid.NewGuid().ToString();
            }
            finally
            {
                if (file != null) file.Position = 0;
            }
        }
    }
}
