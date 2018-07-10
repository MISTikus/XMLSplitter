namespace XMLSplitter.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public string Attributes { get; set; }
        public string Body { get; set; }
        public bool IsClosed { get; set; }
    }
}