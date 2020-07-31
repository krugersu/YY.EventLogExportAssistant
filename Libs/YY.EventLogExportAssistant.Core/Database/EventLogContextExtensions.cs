using System.Linq;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.Database
{
    public static class EventLogContextExtensions
    {
        public static void FillReferencesToSave(this EventLogContext context, InformationSystemsBase system, ReferencesData data)
        {
            if (data.Applications != null)
            {
                foreach (var itemApplication in data.Applications)
                {
                    Applications foundApplication = context.Applications
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemApplication.Name);
                    if (foundApplication == null)
                    {
                        context.Applications.Add(new Applications()
                        {
                            InformationSystemId = system.Id,
                            Name = itemApplication.Name
                        });
                    }
                }
            }
            if (data.Computers != null)
            {
                foreach (var itemComputer in data.Computers)
                {
                    Computers foundComputer = context.Computers
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemComputer.Name);
                    if (foundComputer == null)
                    {
                        context.Computers.Add(new Computers()
                        {
                            InformationSystemId = system.Id,
                            Name = itemComputer.Name
                        });
                    }
                }
            }
            if (data.Events != null)
            {
                foreach (var itemEvent in data.Events)
                {
                    Events foundEvents = context.Events
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemEvent.Name);
                    if (foundEvents == null)
                    {
                        context.Events.Add(new Events()
                        {
                            InformationSystemId = system.Id,
                            Name = itemEvent.Name
                        });
                    }
                }
            }
            if (data.Metadata != null)
            {
                foreach (var itemMetadata in data.Metadata)
                {
                    Metadata foundMetadata = context.Metadata
                        .FirstOrDefault(e => e.InformationSystemId == system.Id
                                             && e.Name == itemMetadata.Name
                                             && e.Uuid == itemMetadata.Uuid);
                    if (foundMetadata == null)
                    {
                        context.Metadata.Add(new Metadata()
                        {
                            InformationSystemId = system.Id,
                            Name = itemMetadata.Name,
                            Uuid = itemMetadata.Uuid
                        });
                    }
                }
            }
            if (data.PrimaryPorts != null)
            {
                foreach (var itemPrimaryPort in data.PrimaryPorts)
                {
                    PrimaryPorts foundPrimaryPort = context.PrimaryPorts
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemPrimaryPort.Name);
                    if (foundPrimaryPort == null)
                    {
                        context.PrimaryPorts.Add(new PrimaryPorts()
                        {
                            InformationSystemId = system.Id,
                            Name = itemPrimaryPort.Name
                        });
                    }
                }
            }
            if (data.SecondaryPorts != null)
            {
                foreach (var itemSecondaryPort in data.SecondaryPorts)
                {
                    SecondaryPorts foundSecondaryPort = context.SecondaryPorts
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemSecondaryPort.Name);
                    if (foundSecondaryPort == null)
                    {
                        context.SecondaryPorts.Add(new SecondaryPorts()
                        {
                            InformationSystemId = system.Id,
                            Name = itemSecondaryPort.Name
                        });
                    }
                }
            }
            if (data.Severities != null)
            {
                foreach (var itemSeverity in data.Severities)
                {
                    Severities foundSeverity = context.Severities
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemSeverity.ToString());
                    if (foundSeverity == null)
                    {
                        context.Severities.Add(new Severities()
                        {
                            InformationSystemId = system.Id,
                            Name = itemSeverity.ToString()
                        });
                    }
                }
            }
            if (data.TransactionStatuses != null)
            {
                foreach (var itemTransactionStatus in data.TransactionStatuses)
                {
                    TransactionStatuses foundTransactionStatus = context.TransactionStatuses
                        .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == itemTransactionStatus.ToString());
                    if (foundTransactionStatus == null)
                    {
                        context.TransactionStatuses.Add(new TransactionStatuses()
                        {
                            InformationSystemId = system.Id,
                            Name = itemTransactionStatus.ToString()
                        });
                    }
                }
            }
            if (data.Users != null)
            {
                foreach (var itemUser in data.Users)
                {
                    Users foundUsers = context.Users
                        .FirstOrDefault(e => e.InformationSystemId == system.Id
                                             && e.Name == itemUser.Name
                                             && e.Uuid == itemUser.Uuid);
                    if (foundUsers == null)
                    {
                        context.Users.Add(new Users()
                        {
                            InformationSystemId = system.Id,
                            Name = itemUser.Name,
                            Uuid = itemUser.Uuid
                        });
                    }
                }
            }
            if (data.WorkServers != null)
            {
                foreach (var itemWorkServer in data.WorkServers)
                {
                    WorkServers foundWorkServer = context.WorkServers
                        .FirstOrDefault(e => e.InformationSystemId == system.Id
                                             && e.Name == itemWorkServer.Name);
                    if (foundWorkServer == null)
                    {
                        context.WorkServers.Add(new WorkServers()
                        {
                            InformationSystemId = system.Id,
                            Name = itemWorkServer.Name
                        });
                    }
                }
            }
        }
    }
}
