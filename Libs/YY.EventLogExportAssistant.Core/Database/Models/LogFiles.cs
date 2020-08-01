using System;
using System.IO;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class LogFiles : CommonLogObject
    {
        #region Public Member

        public string FileName { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime ModificationDate { set; get; }
        public long LastEventNumber { set; get; }
        public string LastCurrentFileReferences { set; get; }
        public string LastCurrentFileData { set; get; }
        public long? LastStreamPosition { set; get; }

        #endregion

        #region Constructors

        public LogFiles()
        {
        }
        public LogFiles(InformationSystemsBase system, FileInfo logFileInfo, EventLogPosition position)
        {
            InformationSystemId = system.Id;
            FileName = logFileInfo.Name;
            CreateDate = logFileInfo.CreationTimeUtc;
            ModificationDate = logFileInfo.LastWriteTimeUtc;
            LastCurrentFileData = position.CurrentFileData;
            LastCurrentFileReferences = position.CurrentFileReferences;
            LastEventNumber = position.EventNumber;
            LastStreamPosition = position.StreamPosition;
        }

        #endregion
    }
}
