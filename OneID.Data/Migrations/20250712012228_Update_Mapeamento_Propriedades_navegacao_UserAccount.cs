using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Mapeamento_Propriedades_navegacao_UserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tb_oneid_user_roles",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tb_oneid_user_claims",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_roles_UserId",
                table: "tb_oneid_user_roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_roles",
                column: "UserId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_user_accounts_UserId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_roles_UserId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tb_oneid_user_claims");
        }
    }
}
