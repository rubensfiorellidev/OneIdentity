using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Refresh_Tokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "tb_oneid_refresh_web_token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedFromIp",
                table: "tb_oneid_refresh_web_token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "tb_oneid_refresh_web_token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "tb_oneid_refresh_web_token",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "tb_oneid_refresh_web_token");

            migrationBuilder.DropColumn(
                name: "CreatedFromIp",
                table: "tb_oneid_refresh_web_token");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "tb_oneid_refresh_web_token");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "tb_oneid_refresh_web_token");
        }
    }
}
