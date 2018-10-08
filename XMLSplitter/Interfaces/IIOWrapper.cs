namespace XMLSplitter.Interfaces
{
    public interface IIOWrapper : IFileReader, IFileWriter
    {
        bool DirectoryExists(string directory);
        void CreateDirectory(string directory);
        string GetFileNameWithoutExtension(string fileName);
    }
}