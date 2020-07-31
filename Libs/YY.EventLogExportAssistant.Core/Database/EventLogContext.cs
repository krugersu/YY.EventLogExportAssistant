using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.Database
{
    public class EventLogContext : DbContext
    {
        #region Private Properties

        private readonly IEventLogContextExtensionActions _extensionActions;
        private readonly DBMSType _DBMSType;

        #endregion

        #region Public Properties

        public DbSet<InformationSystems> InformationSystems { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Computers> Computers { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<PrimaryPorts> PrimaryPorts { get; set; }
        public DbSet<RowData> RowsData { get; set; }
        public DbSet<SecondaryPorts> SecondaryPorts { get; set; }
        public DbSet<Severities> Severities { get; set; }
        public DbSet<TransactionStatuses> TransactionStatuses { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<WorkServers> WorkServers { get; set; }
        public DbSet<LogFiles> LogFiles { get; set; }

        #endregion

        #region Constructor

        public EventLogContext(IEventLogContextExtensionActions extensionActions, DBMSType DBMSType)
        {
            _extensionActions = extensionActions;
            _DBMSType = DBMSType;

            Database.EnsureCreated();
            _extensionActions.AdditionalInitializationActions(Database);
        }
        public EventLogContext(DbContextOptions<EventLogContext> options, IEventLogContextExtensionActions extensionActions, DBMSType DBMSType) : base(options)
        {
            _extensionActions = extensionActions;

            Database.EnsureCreated();
            _extensionActions.AdditionalInitializationActions(Database);
        }

        #endregion

        #region Private Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            CheckSettings();

            _extensionActions.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CheckSettings();

            _extensionActions.OnModelCreating(
                modelBuilder,
                out var standardBehaviorChanged);

            if(standardBehaviorChanged)
                return;;

            modelBuilder.Entity<InformationSystems>()
                 .HasKey(b => new { b.Id });
            modelBuilder.Entity<InformationSystems>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            InitializeStandardEntity<Applications>(modelBuilder);
            InitializeStandardEntity<Computers>(modelBuilder);
            InitializeStandardEntity<Events>(modelBuilder);
            InitializeStandardEntity<Metadata>(modelBuilder);
            InitializeStandardEntity<PrimaryPorts>(modelBuilder);
            InitializeStandardEntity<SecondaryPorts>(modelBuilder);
            InitializeStandardEntity<Severities>(modelBuilder);
            InitializeStandardEntity<TransactionStatuses>(modelBuilder);
            InitializeStandardEntity<Users>(modelBuilder);
            InitializeStandardEntity<WorkServers>(modelBuilder);
            InitializeStandardEntity<LogFiles>(modelBuilder);

            modelBuilder.Entity<RowData>()
                .HasKey(b => new { b.InformationSystemId, b.Period, b.Id });
            modelBuilder.Entity<RowData>()
                .HasIndex(i => new { i.InformationSystemId, i.UserId, i.Period });
            modelBuilder.Entity<RowData>()
                .HasIndex(i => new { i.InformationSystemId, i.DataUUID });

            if (_DBMSType != DBMSType.SQLServer)
            {
                modelBuilder.Entity<InformationSystems>()
                    .HasIndex(b => new { b.Id })
                    .IsUnique();

                AddExplicitStandardIndex<Applications>(modelBuilder);
                AddExplicitStandardIndex<Computers>(modelBuilder);
                AddExplicitStandardIndex<Events>(modelBuilder);
                AddExplicitStandardIndex<Metadata>(modelBuilder);
                AddExplicitStandardIndex<PrimaryPorts>(modelBuilder);
                AddExplicitStandardIndex<SecondaryPorts>(modelBuilder);
                AddExplicitStandardIndex<Severities>(modelBuilder);
                AddExplicitStandardIndex<Users>(modelBuilder);
                AddExplicitStandardIndex<WorkServers>(modelBuilder);

                modelBuilder.Entity<LogFiles>()
                    .HasIndex(b => new { b.InformationSystemId, b.FileName, b.CreateDate, b.Id })
                    .IsUnique();

                modelBuilder.Entity<RowData>()
                    .HasIndex(b => new { b.InformationSystemId, b.Period, b.Id })
                    .IsUnique();
            }
        }

        private void InitializeStandardEntity<T>(ModelBuilder modelBuilder) where T : CommonLogObject
        {
            modelBuilder.Entity<T>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<T>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
        private void AddExplicitStandardIndex<T>(ModelBuilder modelBuilder) where T : CommonLogObject
        {
            modelBuilder.Entity<Applications>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();
        }
        private void CheckSettings()
        {
            if (_extensionActions == null)
                throw new Exception("Ошибка инициализации модуля переопределения поведения иницилаизации базы данных: IEventLogContextExtensionActions.");
        }

        #endregion
    }
}
