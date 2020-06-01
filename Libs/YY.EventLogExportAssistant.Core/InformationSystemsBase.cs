namespace YY.EventLogExportAssistant
{
    public class InformationSystemsBase
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
