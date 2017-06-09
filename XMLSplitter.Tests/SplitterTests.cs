using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;

namespace XMLSplitter.Tests
{
    public class SplitterTests
    {
        [Fact]
        public void SaveSplittedShouldSaveSplittedDocumentsByPassedSize()
        {
            var files = new Dictionary<string, string>();

            var fileReaderMock = new Mock<IFileReader>();
            fileReaderMock.Setup(c => c.Read(It.IsAny<string>())).Returns(XmlStorage.GetXml());
            var fileWriterMock = new Mock<IFileWriter>();
            fileWriterMock.Setup(c => c.Write(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((f, s) => files.Add(f, s));

            var splitter = new Splitter(fileReaderMock.Object, fileWriterMock.Object);

            splitter.SaveSplitted(string.Empty, 10, string.Empty);

            files.Should().NotBeEmpty();
        }
    }
}