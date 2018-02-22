using FileSpliter.Models;
using NUnit.Framework;
using System;
using System.IO;

namespace FileSpliter.BLL.Tests
{
    [TestFixture]
    public class StreamProviderTests
    {
        /// <summary>
        /// тестируемая система
        /// </summary>
        private StreamProvider SUT;

        /// <summary>
        /// установка нового
        /// экземпляра провайдера
        /// </summary>
        private void SetSUT()
        {
            SUT = new StreamProvider();
        }

        /// <summary>
        /// создание массива байтов,
        /// заполненного нулями
        /// </summary>
        /// <param name="len">длина массива</param>
        /// <returns>массив байтов</returns>
        private static byte[] GetBuffer(int len)
        {
            //создаем пустой массив размера
            //len и заполняем его нулями
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; i++)
                buffer[i] = 0;
            return buffer;
        }
        
        /// <summary>
        /// проверка размера потока после склеивания
        /// </summary>
        /// <param name="partsLens">размеры частей</param>
        [TestCase(3)]
        [TestCase(1,5,3,10,18)]
        [TestCase(92,44)]
        public void MergeStreams_VariousParts_ChecksSumLength(params int[] partsLens)
        {
            SetSUT();

            //создаем массив частей, что будут склеены
            FilePart[] parts = new FilePart[partsLens.Length];
            int sumLen = 0;

            //заполняем его массивами,
            //размеры которых указаны в параметрах
            for(int i=0;i<parts.Length;i++)
            {
                parts[i] = new FilePart();
                parts[i].DataBytesArray = GetBuffer(partsLens[i]);
                sumLen += partsLens[i];
            }

            //получаем поток и сверяем сумму
            Stream stream = SUT.MergeStreams(parts);
            Assert.AreEqual(sumLen, stream.Length);
        }

        /// <summary>
        /// проверка имени файла в его частях
        /// </summary>
        [Test]
        public void SplitStream_SomeFileName_ChecksItInFileParts()
        {
            SetSUT();
            //создаем обычный поток и делим его
            int partsCount = 5;
            string fileName = "ab.txt";
            Models.File file = SUT.SplitStream(
                new MemoryStream(GetBuffer(partsCount+1)), partsCount, fileName);

            //проверяем имена
            foreach (FilePart part in file.FileParts)
                Assert.AreEqual(fileName, part.SummaryInfo.FileName);
        }

        /// <summary>
        /// проверка выброса исключения при
        /// несоответствии размера потока
        /// и количества разбиваемых частей
        /// </summary>
        [Test]
        public void SplitStream_PartsCountGreaterThanLength_ThrowsException()
        {
            SetSUT();
            int len = 10;
            Assert.Throws<ArgumentException>(
                () => SUT.SplitStream(
                    new MemoryStream(GetBuffer(len)), len + 10, "av"));
        }

        /// <summary>
        /// проверяем количество частей после разбиения
        /// </summary>
        /// <param name="partsCount">количество частей</param>
        [TestCase(1)]
        [TestCase(7)]
        [TestCase(10000)]
        public void SplitStream_VariousPartsCount_ChecksFilePartsCount(int partsCount)
        {
            SetSUT();
            Models.File file = SUT.SplitStream(
                new MemoryStream(GetBuffer(partsCount+1)), partsCount, "ab.txt");
            Assert.AreEqual(partsCount, file.FileParts.Count);
        }
    }
}
