using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Account_Admission_Stagin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitleName",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "JobTitleName",
                table: "tb_oneid_account_admission_staging");
        }
    }
}
