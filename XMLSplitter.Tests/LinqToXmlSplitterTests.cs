using System.Collections.Generic;
using FluentAssertions;
using Moq;
using XMLSplitter.Interfaces;
using Xunit;

namespace XMLSplitter.Tests
{
    public class LinqToXmlSplitterTests
    {
        [Fact]
        public void SaveSplittedShouldSaveSplittedDocumentsByPassedSize()
        {
            var files = new Dictionary<string, string>();

            var fileReaderMock = new Mock<IFileReader>();
            fileReaderMock.Setup(c => c.FileReadAllText(It.IsAny<string>())).Returns(XmlStorage.TestXml);
            var fileWriterMock = new Mock<IFileWriter>();
            fileWriterMock.Setup(c => c.FileWriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((f, s) => files.Add(f, s));
            var ioWrapperMoq = new Mock<IIOWrapper>();

            var splitter = new LinqToXmlSplitter(fileReaderMock.Object, fileWriterMock.Object, ioWrapperMoq.Object);

            splitter.SaveSplitted(string.Empty, 10, string.Empty);

            files.Should().NotBeEmpty();
        }
    }
}