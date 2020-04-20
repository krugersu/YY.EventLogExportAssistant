using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YY.EventLogExportAssistant.SQLServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InformationSystems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => new { x.InformationSystemId, x.id });
                    table.ForeignKey(
                        name: "FK_Applications_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Computers",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computers", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_Computers_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_Events_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Uuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_Metadata_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrimaryPorts",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryPorts", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_PrimaryPorts_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecondaryPorts",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondaryPorts", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_SecondaryPorts_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Severities",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Severities", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_Severities_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionStatuses",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatuses", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_TransactionStatuses_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Uuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_Users_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkServers",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkServers", x => new { x.InformationSystemId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkServers_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RowsData",
                columns: table => new
                {
                    InformationSystemId = table.Column<long>(nullable: false),
                    Period = table.Column<DateTimeOffset>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    SeverityInformationSystemId = table.Column<long>(nullable: true),
                    SeverityId = table.Column<long>(nullable: true),
                    ConnectId = table.Column<long>(nullable: true),
                    Session = table.Column<long>(nullable: true),
                    TransactionStatusInformationSystemId = table.Column<long>(nullable: true),
                    TransactionStatusId = table.Column<long>(nullable: true),
                    TransactionDate = table.Column<DateTime>(nullable: true),
                    TransactionId = table.Column<long>(nullable: true),
                    UserInformationSystemId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ComputerInformationSystemId = table.Column<long>(nullable: true),
                    ComputerId = table.Column<long>(nullable: true),
                    ApplicationInformationSystemId = table.Column<long>(nullable: true),
                    Applicationid = table.Column<long>(nullable: true),
                    EventInformationSystemId = table.Column<long>(nullable: true),
                    EventId = table.Column<long>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    MetadataInformationSystemId = table.Column<long>(nullable: true),
                    MetadataId = table.Column<long>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    DataUUID = table.Column<string>(nullable: true),
                    DataPresentation = table.Column<string>(nullable: true),
                    WorkServerInformationSystemId = table.Column<long>(nullable: true),
                    WorkServerId = table.Column<long>(nullable: true),
                    PrimaryPortInformationSystemId = table.Column<long>(nullable: true),
                    PrimaryPortId = table.Column<long>(nullable: true),
                    SecondaryPortInformationSystemId = table.Column<long>(nullable: true),
                    SecondaryPortId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowsData", x => new { x.InformationSystemId, x.Period, x.Id });
                    table.ForeignKey(
                        name: "FK_RowsData_InformationSystems_InformationSystemId",
                        column: x => x.InformationSystemId,
                        principalTable: "InformationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsData_Applications_ApplicationInformationSystemId_Applicationid",
                        columns: x => new { x.ApplicationInformationSystemId, x.Applicationid },
                        principalTable: "Applications",
                        principalColumns: new[] { "InformationSystemId", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_Computers_ComputerInformationSystemId_ComputerId",
                        columns: x => new { x.ComputerInformationSystemId, x.ComputerId },
                        principalTable: "Computers",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_Events_EventInformationSystemId_EventId",
                        columns: x => new { x.EventInformationSystemId, x.EventId },
                        principalTable: "Events",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_Metadata_MetadataInformationSystemId_MetadataId",
                        columns: x => new { x.MetadataInformationSystemId, x.MetadataId },
                        principalTable: "Metadata",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_PrimaryPorts_PrimaryPortInformationSystemId_PrimaryPortId",
                        columns: x => new { x.PrimaryPortInformationSystemId, x.PrimaryPortId },
                        principalTable: "PrimaryPorts",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_SecondaryPorts_SecondaryPortInformationSystemId_SecondaryPortId",
                        columns: x => new { x.SecondaryPortInformationSystemId, x.SecondaryPortId },
                        principalTable: "SecondaryPorts",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_Severities_SeverityInformationSystemId_SeverityId",
                        columns: x => new { x.SeverityInformationSystemId, x.SeverityId },
                        principalTable: "Severities",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_TransactionStatuses_TransactionStatusInformationSystemId_TransactionStatusId",
                        columns: x => new { x.TransactionStatusInformationSystemId, x.TransactionStatusId },
                        principalTable: "TransactionStatuses",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_Users_UserInformationSystemId_UserId",
                        columns: x => new { x.UserInformationSystemId, x.UserId },
                        principalTable: "Users",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RowsData_WorkServers_WorkServerInformationSystemId_WorkServerId",
                        columns: x => new { x.WorkServerInformationSystemId, x.WorkServerId },
                        principalTable: "WorkServers",
                        principalColumns: new[] { "InformationSystemId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_InformationSystemId_id",
                table: "Applications",
                columns: new[] { "InformationSystemId", "id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Computers_InformationSystemId_Id",
                table: "Computers",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_InformationSystemId_Id",
                table: "Events",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InformationSystems_Id",
                table: "InformationSystems",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_InformationSystemId_Id",
                table: "Metadata",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrimaryPorts_InformationSystemId_Id",
                table: "PrimaryPorts",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_ApplicationInformationSystemId_Applicationid",
                table: "RowsData",
                columns: new[] { "ApplicationInformationSystemId", "Applicationid" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_ComputerInformationSystemId_ComputerId",
                table: "RowsData",
                columns: new[] { "ComputerInformationSystemId", "ComputerId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_EventInformationSystemId_EventId",
                table: "RowsData",
                columns: new[] { "EventInformationSystemId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_MetadataInformationSystemId_MetadataId",
                table: "RowsData",
                columns: new[] { "MetadataInformationSystemId", "MetadataId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_PrimaryPortInformationSystemId_PrimaryPortId",
                table: "RowsData",
                columns: new[] { "PrimaryPortInformationSystemId", "PrimaryPortId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_SecondaryPortInformationSystemId_SecondaryPortId",
                table: "RowsData",
                columns: new[] { "SecondaryPortInformationSystemId", "SecondaryPortId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_SeverityInformationSystemId_SeverityId",
                table: "RowsData",
                columns: new[] { "SeverityInformationSystemId", "SeverityId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_TransactionStatusInformationSystemId_TransactionStatusId",
                table: "RowsData",
                columns: new[] { "TransactionStatusInformationSystemId", "TransactionStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_UserInformationSystemId_UserId",
                table: "RowsData",
                columns: new[] { "UserInformationSystemId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_WorkServerInformationSystemId_WorkServerId",
                table: "RowsData",
                columns: new[] { "WorkServerInformationSystemId", "WorkServerId" });

            migrationBuilder.CreateIndex(
                name: "IX_RowsData_InformationSystemId_Period_Id",
                table: "RowsData",
                columns: new[] { "InformationSystemId", "Period", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecondaryPorts_InformationSystemId_Id",
                table: "SecondaryPorts",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Severities_InformationSystemId_Id",
                table: "Severities",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionStatuses_InformationSystemId_Id",
                table: "TransactionStatuses",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_InformationSystemId_Id",
                table: "Users",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkServers_InformationSystemId_Id",
                table: "WorkServers",
                columns: new[] { "InformationSystemId", "Id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RowsData");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Computers");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropTable(
                name: "PrimaryPorts");

            migrationBuilder.DropTable(
                name: "SecondaryPorts");

            migrationBuilder.DropTable(
                name: "Severities");

            migrationBuilder.DropTable(
                name: "TransactionStatuses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkServers");

            migrationBuilder.DropTable(
                name: "InformationSystems");
        }
    }
}
