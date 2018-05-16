using FileSpliter.Models;
using NUnit.Framework;
using System;
using System.IO;

namespace FileSpliter.BLL.Tests
{
    [TestFixture]
    public class StreamProviderTests
    {
        private const string FileNameExample = "ab.txt";

        /// <summary>
        /// system for test
        /// </summary>
        private StreamProvider SUT;

        /// <summary>
        /// set new instance of StreamProvider 
        /// to SUT field
        /// </summary>
        [SetUp]
        public void Setup()
        {
            SUT = new StreamProvider();
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
                sumLength += partsSizes[i];
            }

            //get stream and check sum
            Stream stream = SUT.MergeStreams(parts);
            Assert.AreEqual(sumLength, stream.Length);
        }

        /// <summary>
        /// check file name in file parts
        /// </summary>
        //[Test]
        //public void SplitStream_SomeFileName_OriginalFileNameAndNamesInPartsShouldBeEqual()
        //{

        //    //create simple stream and split it
        //    int partsCount = 5;
        //    string fileName = FileNameExample;
        //    Models.File file = SUT.SplitFile(
        //        new MemoryStream(GetBuffer(partsCount+1)), partsCount, fileName);

        //    //check names
        //    foreach (FilePart part in file.FileParts)
        //        Assert.AreEqual(fileName, part.SummaryInfo.FileName);
        //}

        /// <summary>
        /// test exception throwing when
        /// stream size lower than 
        /// number of parts
        /// </summary>
        //[Test]
        //public void SplitStream_PartsCountGreaterThanLength_ThrowsException()
        //{
        //    int len = 10;
        //    Assert.Throws<ArgumentException>(
        //        () => SUT.SplitFile(
        //            new MemoryStream(GetBuffer(len)), len + 10, "av"));
        //}

        /// <summary>
        /// test number of parts after splitting
        /// </summary>
        /// <param name="partsCount">number of parts</param>
        //[TestCase(1)]
        //[TestCase(7)]
        //[TestCase(10000)]
        //public void SplitStream_VariousPartsCount_NumberOfPartsInParameterAndInResultShouldBeEqual(int partsCount)
        //{
        //    Models.File file = SUT.SplitFile(
        //        new MemoryStream(GetBuffer(partsCount+1)), partsCount, FileNameExample);
        //    Assert.AreEqual(partsCount, file.FileParts.Count);
        //}
    }
}
