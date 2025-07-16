using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameFirstnameLastnameInAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_oneid_automatic_admission_audit",
                table: "tb_oneid_automatic_admission_audit");

            migrationBuilder.RenameTable(
                name: "tb_oneid_automatic_admission_audit",
                newName: "tb_oneid_admission_audit");

            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "tb_oneid_admission_audit",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Firstname",
                table: "tb_oneid_admission_audit",
                newName: "FirstName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_oneid_admission_audit",
                table: "tb_oneid_admission_audit",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_oneid_admission_audit",
                table: "tb_oneid_admission_audit");

            migrationBuilder.RenameTable(
                name: "tb_oneid_admission_audit",
                newName: "tb_oneid_automatic_admission_audit");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "tb_oneid_automatic_admission_audit",
                newName: "Lastname");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "tb_oneid_automatic_admission_audit",
                newName: "Firstname");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_oneid_automatic_admission_audit",
                table: "tb_oneid_automatic_admission_audit",
                column: "Id");
        }
    }
}
