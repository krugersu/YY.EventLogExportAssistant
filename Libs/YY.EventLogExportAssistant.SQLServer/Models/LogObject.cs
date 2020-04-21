namespace YY.EventLogExportAssistant.SQLServer.Models
{
    public abstract class LogObject : CommonLogObject
    {
        public virtual long InformationSystemId { get; set; }
        public virtual InformationSystems InformationSystem { get; set; }
    }
}
