using System.Collections.Generic;
using XMLSplitter.Interfaces;

namespace XMLSplitter.Helpers
{
    public class IOWrapper : IIOWrapper
    {
        public void CreateDirectory(string directory) => System.IO.Directory.CreateDirectory(directory);

        public bool DirectoryExists(string directory) => System.IO.Directory.Exists(directory);

        public string GetFileNameWithoutExtension(string fileName) => System.IO.Path.GetFileNameWithoutExtension(fileName);
        public string FileReadAllText(string filePath) => System.IO.File.ReadAllText(filePath);

        public void FileWriteAllText(string fileName, string data) => System.IO.File.WriteAllText(fileName, data);

        public IEnumerable<char> Read(string filePath)
        {
            using(var stream = System.IO.File.OpenText(filePath))
            {
                while (stream.Peek() >= 0)
                {
                    yield return (char) stream.Read();
                }
            }
        }
    }
}