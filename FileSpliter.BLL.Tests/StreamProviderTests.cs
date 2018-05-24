using FileSpliter.Models;
using NUnit.Framework;
using System;
using System.IO;
using FileSpliter.Interfaces;
using Moq;

namespace FileSpliter.BLL.Tests
{
    [TestFixture]
    public class StreamProviderTests
    {
        private const string FileNameExample = "ab.txt";

        /// <summary>
        /// system for test
        /// </summary>
        private StreamProvider _sut;

        private Mock<IFileHasher> _fileHasher;
        private Mock<IMemoryBufferManager> _memoryBuffer;
        private Mock<IFileSerializator> _fileSerializator;

        /// <summary>
        /// set new instance of StreamProvider 
        /// to SUT field
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _fileHasher = new Mock<IFileHasher>();
            _memoryBuffer = new Mock<IMemoryBufferManager>();
            _fileSerializator = new Mock<IFileSerializator>();
            _sut = new StreamProvider(_fileHasher.Object, _fileSerializator.Object, _memoryBuffer.Object);
        }

        /// <summary>
        /// create byte array, that
        /// filled with zero-values
        /// </summary>
        /// <param name="length">array length</param>
        /// <returns>byte array</returns>
        private static byte[] GetBuffer(int length)
        {
            //create byte array with length size
            //and fill it with zero values
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
                buffer[i] = 0;
            return buffer;
        }
        
        /// <summary>
        /// test stream size after merge
        /// </summary>
        /// <param name="partsSizes">size of parts</param>
        [TestCase(3)]
        [TestCase(1,5,3,10,18)]
        [TestCase(92,44)]
        public void MergeStreams_VariousParts_ShouldReturnStreamWithCorrectBufferLength(params int[] partsSizes)
        {

            //create parts for merge
            FilePart[] parts = new FilePart[partsSizes.Length];
            int sumLength = 0;

            //fill parts with byte arrays,
            //we get size of arrays from test params
            for(int i=0;i<parts.Length;i++)
            {
                parts[i] = new FilePart();
                parts[i].DataBytesArray = GetBuffer(partsSizes[i]);
                parts[i].PartInfo = new FilePartInfo{PartSize = parts[i].DataBytesArray.Length};
                sumLength += partsSizes[i];
            }

            //get stream and check sum
            using (Stream stream = _sut.MergeStreams(parts, FileNameExample))
            {
                Assert.AreEqual(sumLength, stream.Length);
            }
        }

        /// <summary>
        /// check file name in file parts
        /// </summary>
        [Test]
        public void SplitStream_SomeFileName_OriginalFileNameAndNamesInPartsShouldBeEqual()
        {

            //create simple stream and split it
            int partsCount = 5;
            string fileName = FileNameExample;
            Models.File file = _sut.SplitFileAsync("", partsCount, fileName).Result;

            //check names
            foreach (FilePart part in file.FileParts)
                Assert.AreEqual(fileName, part.SummaryInfo.FileName);
        }

        /// <summary>
        /// test exception throwing when
        /// stream size lower than 
        /// number of parts
        /// </summary>
        [Test]
        public void SplitStream_PartsCountGreaterThanLength_ThrowsException()
        {
            int len = 10;
            Assert.Throws<ArgumentException>(async () => await _sut.SplitFileAsync("", len + 10, "av"));
        }

        /// <summary>
        /// test number of parts after splitting
        /// </summary>
        /// <param name="partsCount">number of parts</param>
        [TestCase(1)]
        [TestCase(7)]
        [TestCase(10000)]
        public void SplitStream_VariousPartsCount_NumberOfPartsInParameterAndInResultShouldBeEqual(int partsCount)
        {
            Models.File file = _sut.SplitFileAsync("", partsCount, FileNameExample).Result;
            Assert.AreEqual(partsCount, file.FileParts.Count);
        }
    }
}
