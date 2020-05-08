using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using YY.EventLogExportAssistant.PostgreSQL.Models;

namespace YY.EventLogExportAssistant.PostgreSQL
{
    public class EventLogContext : DbContext
    {
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

        public EventLogContext() : base()
        {
            Database.EnsureCreated();
            AdditionalInitializationActions();
        }

        public EventLogContext(DbContextOptions<EventLogContext> options) : base(options)
        {
            Database.EnsureCreated();
            AdditionalInitializationActions();
        }

        private void AdditionalInitializationActions()
        {
            // TODO
            // Создавать в базе VIEW для просмотра данных в удобном виде
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                string connectinString = Configuration.GetConnectionString("EventLogDatabase");
                optionsBuilder.UseNpgsql(connectinString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InformationSystems>()
                .HasKey(b => new { b.Id });
            modelBuilder.Entity<InformationSystems>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Applications>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Applications>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Computers>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Computers>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Events>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Events>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Metadata>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Metadata>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PrimaryPorts>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<PrimaryPorts>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<SecondaryPorts>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<SecondaryPorts>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Severities>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Severities>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TransactionStatuses>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<TransactionStatuses>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Users>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Users>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<WorkServers>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<WorkServers>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<LogFiles>()
                .HasKey(b => new { b.InformationSystemId, b.FileName, b.CreateDate, b.Id });
            modelBuilder.Entity<LogFiles>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RowData>()
                .HasKey(b => new { b.InformationSystemId, b.Period, b.Id });
            modelBuilder.Entity<RowData>()
                .HasIndex(i => new { i.InformationSystemId, i.UserId, i.Period });
            modelBuilder.Entity<RowData>()
                .HasIndex(i => new { i.InformationSystemId, i.DataUUID });
        }
    }
}
