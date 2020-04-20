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

        public EventLogContext() : base()
        {
            Database.EnsureCreated();
        }

        public EventLogContext(DbContextOptions<EventLogContext> options) : base(options)
        {
            Database.EnsureCreated();
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
                .HasIndex(b => new { b.Id })
                .IsUnique();

            modelBuilder.Entity<Applications>()
                .HasKey(b => new { b.InformationSystemId, b.id });
            modelBuilder.Entity<Applications>()
                .HasIndex(b => new { b.InformationSystemId, b.id })
                .IsUnique();

            modelBuilder.Entity<Computers>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Computers>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<Events>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Events>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<Metadata>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Metadata>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<PrimaryPorts>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<PrimaryPorts>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<RowData>()
                .HasKey(b => new { b.InformationSystemId, b.Period, b.Id });
            modelBuilder.Entity<RowData>()
                .HasIndex(b => new { b.InformationSystemId, b.Period, b.Id })
                .IsUnique();


            modelBuilder.Entity<SecondaryPorts>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<SecondaryPorts>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<Severities>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Severities>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<TransactionStatuses>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<TransactionStatuses>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<Users>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();

            modelBuilder.Entity<WorkServers>()
                .HasKey(b => new { b.InformationSystemId, b.Id });
            modelBuilder.Entity<WorkServers>()
                .HasIndex(b => new { b.InformationSystemId, b.Id })
                .IsUnique();
        }
    }
}
