using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XMLSplitter.Interfaces;

namespace XMLSplitter
{
    public class LinqToXmlSplitter : ISplitter
    {
        private readonly IFileReader reader;
        private readonly IFileWriter writer;
        private readonly IIOWrapper ioWrapper;

        public LinqToXmlSplitter(IFileReader reader, IFileWriter writer, IIOWrapper ioWrapper)
        {
            this.reader = reader;
            this.writer = writer;
            this.ioWrapper = ioWrapper;
        }

        public void SaveSplitted(string sourceFileName, int splittedFileSize, string destinationDirectory)
        {
            if (!this.ioWrapper.DirectoryExists(destinationDirectory))
                this.ioWrapper.CreateDirectory(destinationDirectory);

            var locker = new object();
            Func<bool, string, int, string> getFileName = (wi, s, i) =>
            {
                lock(locker)
                {
                    return $"{destinationDirectory}\\{this.ioWrapper.GetFileNameWithoutExtension(sourceFileName)}_{s}{(wi ? $"_{i}" : "")}.xml";
                }
            };

            string rootName = null;
            IEnumerable<XAttribute> attributes = null;
            var task = Task.Factory.StartNew(() =>
            {
                var doc = XDocument.Load(sourceFileName);
                var root = doc.Root;
                rootName = doc.Root.Name.LocalName;
                attributes = root.Attributes();
            });

            GC.Collect(2);
            Func<XmlFile> getNewFile = () =>
            {
                task.Wait();
                return new XmlFile(rootName, attributes);
            };

            using(var reader = XmlReader.Create(sourceFileName))
            {
                var hierarchy = 0;
                var root = "";
                var parent = "";
                var current = "";

                var toSave = new List<XElement>();
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        root = reader.LocalName;

                        using(var subReader = reader.ReadSubtree())
                        {
                            while (subReader.Read())
                            {
                                if (subReader.IsStartElement())
                                {
                                    if (subReader.LocalName == root)
                                        continue;

                                    parent = subReader.LocalName;

                                    var i = 0;
                                    using(var elementReader = subReader.ReadSubtree())
                                    {
                                        while (elementReader.Read())
                                        {
                                            if (elementReader.IsStartElement())
                                            {
                                                if (elementReader.LocalName == parent)
                                                    continue;

                                                current = subReader.LocalName;

                                                if (XNode.ReadFrom(elementReader) is XElement node)
                                                {
                                                    toSave.Add(node);

                                                    if (XmlFile.GetSize(string.Join("", toSave.Select(x => x.ToString()))) >= splittedFileSize)
                                                    {
                                                        var file = getNewFile();
                                                        toSave.ForEach(file.Add);
                                                        this.writer.FileWriteAllText(getFileName(true, current, i++), file.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}