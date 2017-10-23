namespace DocWorks.GDocFactory.Model
{
    /// <summary>
    /// This class is used to hold the properties as request to Create Google documents(files or Folders)
    /// </summary>
    public class GDriveFile
    {
        public string FileId { get; set; }

        public string FileUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Parent { get; set; }

        public string MimeType { get; set; }

        public string FileContent { get; set; }
    }
}
