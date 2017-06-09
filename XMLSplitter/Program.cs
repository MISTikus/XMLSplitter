using System;
using System.Collections.Generic;

namespace XMLSplitter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Dictionary<ArgumentType, string> arguments = null;
            try
            {
                arguments = ArgumentParser.Parse(args);
                var filesAdapter = new FilesAdapter();
                var splitter = new Splitter(filesAdapter, filesAdapter);
                int size = (int)(decimal.Parse(arguments[ArgumentType.SplittedFileSize]) * 1024m * 1024m * 1.8m);
                splitter.SaveSplitted(arguments[ArgumentType.SourceFileName], size, arguments[ArgumentType.DestinationFolder]);
            }
            catch (Exception e)
            {
                WriteHelp(e);
            }
        }

        private static void WriteHelp(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occured: {e.Message}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"");
            Console.WriteLine($"#\tArguments:");
            Console.WriteLine($"#\t\t-sf\tSource xml file path (ex. -sf:\"c:\\temp\\references.xml\")");
            Console.WriteLine($"#\t\t-d\tDestination folder (ex. -d:\"c:\\temp\\splitted_references\")");
            Console.WriteLine($"#\t\t-s\tMaximum size of splitted xml focuments (MB) (ex. -s:50)");
            Console.WriteLine($"");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }
    }
}