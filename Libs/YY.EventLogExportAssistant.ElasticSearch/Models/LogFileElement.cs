using System;

namespace YY.EventLogExportAssistant.ElasticSearch.Models
{
    public class LogFileElement
    {
        public string Id { get; set; }
        public string InformationSystem { get; set; }
        public string FileName { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime ModificationDate { set; get; }
        public long LastEventNumber { set; get; }
        public string LastCurrentFileReferences { set; get; }
        public string LastCurrentFileData { set; get; }
        public long? LastStreamPosition { set; get; }
    }
}
