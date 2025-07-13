using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeycloakUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.AddColumn<Guid>(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts",
                type: "uuid",
                nullable: true); // ou false se for obrigatório
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.AddColumn<string>(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

    }
}
