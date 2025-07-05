using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ApplicationUser_RemoveNames_AddKeycloakUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "tb_oneid_users");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "tb_oneid_users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "tb_oneid_users");

            migrationBuilder.AlterColumn<string>(
                name: "KeycloakUserId",
                table: "tb_oneid_users",
                type: "varchar",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KeycloakUserId",
                table: "tb_oneid_users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "tb_oneid_users",
                type: "varchar",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "tb_oneid_users",
                type: "varchar",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "tb_oneid_users",
                type: "varchar",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
