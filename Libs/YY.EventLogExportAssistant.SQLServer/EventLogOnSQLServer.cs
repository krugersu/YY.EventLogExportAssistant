namespace YY.EventLogExportAssistant.SQLServer
{
    public class EventLogOnSQLServer : EventLogOnTarget
    {
        private const int _defaultPortion = 1000;
        private int _portion;
        private EventLogContext _context;

        public EventLogOnSQLServer() : this(null, _defaultPortion)
        {
            
        }
        public EventLogOnSQLServer(int portion)
        {
            _portion = portion;
        }
        public EventLogOnSQLServer(EventLogContext context, int portion)
        {
            _portion = portion;
            if(context == null)
                _context = new EventLogContext();
            else
                _context = context;
        }

        public override void Save(CommonLogObject rowData)
        {
            _context.Add(rowData);
        }

        public override void Dispose()
        {
            _context.Dispose();
        }
    }
}
