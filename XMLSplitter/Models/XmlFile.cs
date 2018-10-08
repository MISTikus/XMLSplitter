using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XMLSplitter
{
    public class XmlFile
    {
        private XDocument document;
        private XElement writableNode;
        private XElement root;

        public XmlFile(string rootName, IEnumerable<XAttribute> attributes)
        {
            this.document = new XDocument();
            this.document.Add(new XElement(rootName));
            this.writableNode = document.Root;
            this.writableNode.ReplaceAttributes(attributes);
        }

        public XmlFile(XElement root)
        {
            this.document = new XDocument();
            this.document.Add(root);
            this.document.Root.RemoveNodes();
            this.writableNode = document.Root;
        }

        public void AddNode(string nodeName, string value, IEnumerable<XAttribute> attributes)
        {
            var node = new XElement(nodeName, value);
            node.ReplaceAttributes(attributes);
            this.writableNode.Add(node);
            this.writableNode = node;
        }

        public XmlFile Clone()
        {
            var clone = new XmlFile(this.document.Root);
            clone.Add(this.writableNode);
            clone.ClearWritable();
            return clone;
        }

        private void ClearWritable() => this.writableNode.RemoveNodes();

        public int GetSize() => Encoding.Unicode.GetByteCount(this.document.ToString());

        public static int GetSize(string content) => Encoding.Unicode.GetByteCount(content);

        /// <summary>
        /// Serialize document to xml string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.document.ToString();

        internal void Add(XElement node)
        {
            this.writableNode.Add(node);
            if (this.document.Root == this.writableNode)
                this.writableNode = this.document.Root.Element(node.Name);
        }

        public IEnumerable<XmlFile> Split(int splittedFileSize)
        {
            var file = this.Clone();
            var i = 0;
            var current = this.writableNode;

            foreach (var g in current
                .Elements()
                .Select(n => new { i = i++, n })
                .GroupBy(g => g.i % 100))
            {
                foreach (var node in g)
                    file.Add(node.n);
                if (file.GetSize() >= splittedFileSize)
                {
                    yield return file;
                    file = this.Clone();
                }
            }
        }
    }
}