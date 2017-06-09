using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XMLSplitter
{
    public class Splitter
    {
        private IFileReader reader;
        private IFileWriter writer;

        public Splitter(IFileReader reader, IFileWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void SaveSplitted(string sourceFileName, int splittedFileSize, string destinationDirectory)
        {
            var fileContent = this.reader.Read(sourceFileName);
            var files = new ConcurrentDictionary<string, XmlFile>();

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            var doc = XDocument.Parse(fileContent);
            var root = doc.Root;

            Func<XmlFile> getNewFile = () => new XmlFile(root.Name.LocalName, root.Attributes());
            int i = 1;
            Func<bool, string, string> getFileName = (wi, s) => $"{destinationDirectory}\\{Path.GetFileNameWithoutExtension(sourceFileName)}_{s}{(wi ? $"_{i++}" : "")}.xml";

            foreach (var node in root.Elements().AsParallel())
            {
                var file = getNewFile();
                file.Add(node);
                if (file.GetSize() >= splittedFileSize)
                {
                    foreach (var f in file.Split(splittedFileSize))
                        files.TryAdd(getFileName(true, node.Name.LocalName), f);
                }
                else
                    files.TryAdd(getFileName(false, node.Name.LocalName), file);
            }

            foreach (var key in files.Keys)
            {
                this.writer.Write(key, files[key].ToString());
            }
        }
    }
}