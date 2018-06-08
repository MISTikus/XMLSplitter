using System.IO;

namespace XMLSplitter
{
    public class FilesAdapter : IFileReader, IFileWriter
    {
        public string Read(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void Write(string fileName, string data)
        {
            File.WriteAllText(fileName, data);
        }
    }
}