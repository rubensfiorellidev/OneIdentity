using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "tb_oneid_user_accounts",
                newName: "DepartmentName");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "tb_oneid_user_accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitleName",
                table: "tb_oneid_user_accounts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropColumn(
                name: "JobTitleName",
                table: "tb_oneid_user_accounts");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "tb_oneid_user_accounts",
                newName: "Department");
        }
    }
}
