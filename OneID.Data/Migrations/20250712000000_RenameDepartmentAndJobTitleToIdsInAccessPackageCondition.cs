using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameDepartmentAndJobTitleToIdsInAccessPackageCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "tb_oneid_access_package_condition",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitleId",
                table: "tb_oneid_access_package_condition",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "tb_oneid_access_package_condition");

            migrationBuilder.DropColumn(
                name: "JobTitleId",
                table: "tb_oneid_access_package_condition");
        }
    }
}
