using System.Collections.Generic;

namespace XMLSplitter.Interfaces
{
    public interface IFileReader
    {
        string FileReadAllText(string filePath);
        IEnumerable<char> Read(string filePath);
    }
}