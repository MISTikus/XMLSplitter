using System;
using System.Collections.Generic;

namespace XMLSplitter
{
    public class ArgumentParser
    {
        public static Dictionary<ArgumentType, string> Parse(string[] args)
        {
            var result = new Dictionary<ArgumentType, string>();
            foreach (var a in args)
            {
                var split = a.Split(new[] { ':' }, 2);
                ArgumentType argType = ArgumentType.Default;
                switch (split[0])
                {
                    case "-sf": argType = ArgumentType.SourceFileName; break;
                    case "-s": argType = ArgumentType.SplittedFileSize; break;
                    case "-d": argType = ArgumentType.DestinationFolder; break;
                    default:
                        throw new ArgumentException($"Not registered argument passed: {split[0]}.");
                }
                if (argType != ArgumentType.Default)
                    result.Add(argType, split[1]);
            }
            return result;
        }
    }
}