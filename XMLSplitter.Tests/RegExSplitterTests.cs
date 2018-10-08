using System.Collections.Generic;
using FluentAssertions;
using Moq;
using XMLSplitter.Interfaces;
using XMLSplitter.Splitters;
using Xunit;

namespace XMLSplitter.Tests
{
    public class RegExSplitterTests
    {
        [Fact]
        public void SaveSplittedShouldSaveSplittedDocumentsByPassedSize()
        {
            var files = new Dictionary<string, string>();

            var ioWrapperMoq = new Mock<IIOWrapper>();
            ioWrapperMoq.Setup(c => c.FileReadAllText(It.IsAny<string>())).Returns(XmlStorage.TestXml);
            ioWrapperMoq.Setup(c => c.Read(It.IsAny<string>())).Returns(XmlStorage.TestXml);
            ioWrapperMoq.Setup(c => c.FileWriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((f, s) => files.Add(f, s));

            var splitter = new RegExSplitter(ioWrapperMoq.Object);

            splitter.SaveSplitted(string.Empty, 10, string.Empty);

            files.Should().NotBeEmpty();
        }
    }
}