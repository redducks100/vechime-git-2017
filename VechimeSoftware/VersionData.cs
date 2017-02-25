using MongoDB.Bson;


namespace VechimeSoftware
{
    public class VersionData
    {
        public ObjectId _id { get; set; }
        public string Version { get; set; }
        public string Info { get; set; }
        public string MD5 { get; set; }
        public string DownloadURL { get; set; }
        public long UpdateSize { get; set; }
    }
}
