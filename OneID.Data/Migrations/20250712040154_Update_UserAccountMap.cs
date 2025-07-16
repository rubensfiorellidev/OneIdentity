using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserAccountMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_accounts_DepartmentId",
                table: "tb_oneid_user_accounts",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_accounts_JobTitleId",
                table: "tb_oneid_user_accounts",
                column: "JobTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_accounts_tb_oneid_departments_DepartmentId",
                table: "tb_oneid_user_accounts",
                column: "DepartmentId",
                principalTable: "tb_oneid_departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_accounts_tb_oneid_job_titles_JobTitleId",
                table: "tb_oneid_user_accounts",
                column: "JobTitleId",
                principalTable: "tb_oneid_job_titles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_accounts_tb_oneid_departments_DepartmentId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_accounts_tb_oneid_job_titles_JobTitleId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_accounts_DepartmentId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_accounts_JobTitleId",
                table: "tb_oneid_user_accounts");
        }
    }
}
