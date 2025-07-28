using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_RefreshToken_Hashs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "tb_oneid_refresh_web_token");

            migrationBuilder.DropColumn(
                name: "UserUpn",
                table: "tb_oneid_refresh_web_token");

            migrationBuilder.RenameColumn(
                name: "CreatedFromIp",
                table: "tb_oneid_refresh_web_token",
                newName: "TokenSalt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenSalt",
                table: "tb_oneid_refresh_web_token",
                newName: "CreatedFromIp");

            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "tb_oneid_refresh_web_token",
                type: "VARCHAR",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpn",
                table: "tb_oneid_refresh_web_token",
                type: "VARCHAR",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
