using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Ajuste_Tabela_UserClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_UserAccounts",
                table: "tb_oneid_user_claims",
                column: "UserAccountId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_UserAccounts",
                table: "tb_oneid_user_claims");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tb_oneid_user_claims",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_claims",
                column: "UserAccountId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id");
        }
    }
}
