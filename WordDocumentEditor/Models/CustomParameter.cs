namespace WordDocumentEditor.Models
{
    public class CustomParameter
    {
        public int DocumentId { get; set; }
        public string fileName
        {
            get;
            set;
        }
        public string documentData
        {
            get;
            set;
        }
    }


    public class DocumentInfo
    {
        public int FileIndex { get; set; }
        public string FileName { get; set; }
    }

    public class CustomParams
    {
        public string fileName
        {
            get;
            set;
        }
    }


    public class PathInfo
    {
        public string path { get; set; }
    }
}
