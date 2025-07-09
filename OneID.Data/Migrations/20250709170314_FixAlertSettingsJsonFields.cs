using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAlertSettingsJsonFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarningRecipients",
                table: "tb_oneid_alert_settings",
                newName: "WarningRecipientsJson");

            migrationBuilder.RenameColumn(
                name: "InfoRecipients",
                table: "tb_oneid_alert_settings",
                newName: "InfoRecipientsJson");

            migrationBuilder.RenameColumn(
                name: "CriticalRecipients",
                table: "tb_oneid_alert_settings",
                newName: "CriticalRecipientsJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarningRecipientsJson",
                table: "tb_oneid_alert_settings",
                newName: "WarningRecipients");

            migrationBuilder.RenameColumn(
                name: "InfoRecipientsJson",
                table: "tb_oneid_alert_settings",
                newName: "InfoRecipients");

            migrationBuilder.RenameColumn(
                name: "CriticalRecipientsJson",
                table: "tb_oneid_alert_settings",
                newName: "CriticalRecipients");
        }
    }
}
