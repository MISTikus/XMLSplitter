namespace XMLSplitter.Interfaces
{
    public interface ISplitter
    {
        void SaveSplitted(string sourceFileName, int splittedFileSize, string destinationDirectory);
    }
}