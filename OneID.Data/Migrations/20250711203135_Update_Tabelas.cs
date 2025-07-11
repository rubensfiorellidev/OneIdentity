using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Tabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfHire",
                table: "tb_oneid_user_accounts",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "tb_oneid_user_accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "tb_oneid_user_accounts");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "tb_oneid_user_accounts",
                newName: "DateOfHire");
        }
    }
}
