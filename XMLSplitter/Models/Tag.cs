using System.Text;
using XMLSplitter.Helpers;

namespace XMLSplitter.Models
{
    public struct Tag
    {
        public string Name { get; set; }
        public string Attributes { get; set; }
        public string Body { get; set; }
        public bool IsClosed { get; set; }

        public bool IsDefault => string.IsNullOrWhiteSpace(Name);

        public override string ToString()
        {
            return IsClosed
                ? $"<{Name}{Attributes.AddSpaceIfNotEmpty()} />"
                : string.IsNullOrWhiteSpace(Body)
                    ? $"<{Name}{Attributes.AddSpaceIfNotEmpty()} />"
                    : $"<{Name}{Attributes.AddSpaceIfNotEmpty()}>{Body}</{Name}>";
        }

        public int GetSize() => Encoding.Unicode.GetByteCount(ToString());
    }
}