using FileSpliter.BLL.Services;
using FileSpliter.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace FileSpliter.BLL.Tests
{
    [TestFixture]
    public class FileServiceTests
    {
        private const string FileNameExample = "ab.txt";

        private Mock<IFileSerializator> fileSerializator;
        private Mock<IStreamProvider> streamProvider;

        private FileService SUT;

        [SetUp]
        public void Setup()
        {
            fileSerializator = new Mock<IFileSerializator>();
            streamProvider = new Mock<IStreamProvider>();
            SUT = new FileService(fileSerializator.Object,streamProvider.Object);
        }

        [Ignore("file system dependency in split method exists")]
        public void Split_EmptyFile_ShouldReturnTheSameFile()
        {
            var file = new Models.File();

            streamProvider.Setup(m => m.SplitStream(
                It.Is<Stream>(s => true),
                It.Is<int>(i => true),
                It.Is<string>(s => s.EndsWith(FileNameExample)))).Returns(file);

            Assert.AreEqual(file, SUT.Split(FileNameExample, 2));
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(15)]
        public void SaveParts_FileWithSeveralParts_ShouldCallWritePartForEachPart(int partsCount)
        {
            var fileParts = new List<Models.FilePart>();
            for (int i = 0; i < partsCount; i++)
                fileParts.Add(new Models.FilePart());

            var file = new Models.File()
            {
                FileParts = fileParts
            };

            SUT.SaveParts(file, FileNameExample);

            fileSerializator.Verify(m => m.WriteFilePart(
                It.Is<Models.FilePart>(f => true), 
                It.Is<string>(s => true)), 
                Times.Exactly(partsCount));
        }
    }
}
