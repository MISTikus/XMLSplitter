using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XMLSplitter.Interfaces;
using XMLSplitter.Models;

namespace XMLSplitter.Splitters
{
    public class RegExSplitter : ISplitter
    {
        private readonly IIOWrapper ioWrapper;
        private readonly Regex firstRow;
        private readonly Regex tagBegin;
        private readonly Regex tagEnd;

        public RegExSplitter(IIOWrapper ioWrapper)
        {
            this.ioWrapper = ioWrapper;
            this.firstRow = new Regex(firstRowPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            this.tagBegin = new Regex(tagBeginPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            this.tagEnd = new Regex(tagEndPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public void SaveSplitted(string sourceFileName, int splittedFileSize, string destinationDirectory)
        {
            var locker = new object();

            Func<bool, string, int, string> getFileName = (indexed, tagName, index) =>
            {
                lock (locker)
                {
                    return $"{destinationDirectory}\\{this.ioWrapper.GetFileNameWithoutExtension(sourceFileName)}_{tagName}{(indexed ? $"_{index}" : "")}.xml";
                }
            };

            var firstRow = string.Empty;
            var hierarchy = new Stack<Tag>();
            var buffer = string.Empty;
            var carret = default(Tag);

            foreach (var c in this.ioWrapper.Read(sourceFileName))
            {
                buffer += c;
                if (string.IsNullOrWhiteSpace(firstRow))
                {
                    if (!IsMatch(this.firstRow, buffer))
                        continue;
                    firstRow = buffer;
                    buffer = string.Empty;
                    continue;
                }

                if (carret.IsDefault)
                {
                    if (!IsMatch(this.tagBegin, buffer))
                        continue;
                    carret = ParseTag(buffer);
                    buffer = string.Empty;
                    continue;
                }

                if (IsMatch(this.tagEnd, buffer))
                {
                    carret.Body += ParseBody(buffer);
                    buffer = string.Empty;
                    var temp = carret;
                    if (hierarchy.TryPop(out carret))
                    {
                        carret.Body += temp.ToString();
                        continue;
                    }
                    else
                    {
                        FinalizeFile(firstRow, temp);
                        break;
                    }
                }

                if (IsMatch(this.tagBegin, buffer))
                {
                    hierarchy.Push(carret);
                    carret = ParseTag(buffer);
                    buffer = string.Empty;
                    continue;
                }
            }
        }

        private void FinalizeFile(string firstRow, Tag temp) => this.ioWrapper.FileWriteAllText("", firstRow + temp.ToString());

        private Tag ParseTag(string buffer)
        {
            var match = this.tagBegin.Match(buffer);
            var tag = new Tag
            {
                Name = match.Groups["name"].Value,
                Attributes = match.Groups["attributes"]?.Value,
                IsClosed = !string.IsNullOrWhiteSpace(match.Groups["isClosed"]?.Value)
            };
            return tag;
        }

        private string ParseBody(string buffer)
        {
            var match = this.tagEnd.Match(buffer);
            var body = match.Groups["body"].Value;
            return body;
        }

        private bool IsMatch(Regex expression, string buffer) => expression.IsMatch(buffer);

        #region patterns

        private const string firstRowPattern = @"<\??xml[^>]*>";
        private const string tagBeginPattern = "<(?<name>[\\w\\d]+)(?<attributes>\\s+[\\w\\d]+=?\"?[\\w\\d]+\"?){0,}(?<isClosed>\\s*\\/)?>";
        private const string tagEndPattern = @"(?<body>.*)<\/(?<name>[\w\d]+)>";

        #endregion patterns
    }
}