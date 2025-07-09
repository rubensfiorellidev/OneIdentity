using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Alert_Notifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "tb_oneid_user_accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "tb_hwid_admission_alert",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CpfHash = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PositionHeldId = table.Column<string>(type: "text", nullable: true),
                    WarningMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_hwid_admission_alert", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_alert_settings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CriticalRecipients = table.Column<List<string>>(type: "jsonb", nullable: false),
                    WarningRecipients = table.Column<List<string>>(type: "jsonb", nullable: false),
                    InfoRecipients = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_alert_settings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_hwid_admission_alert");

            migrationBuilder.DropTable(
                name: "tb_oneid_alert_settings");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "tb_oneid_user_accounts");
        }
    }
}
