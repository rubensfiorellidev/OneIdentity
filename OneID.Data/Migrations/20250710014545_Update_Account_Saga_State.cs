using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Account_Saga_State : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "tb_oneid_account_saga_state",
                newName: "KeycloakData");

            migrationBuilder.AddColumn<string>(
                name: "AccountData",
                table: "tb_oneid_account_saga_state",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountData",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.RenameColumn(
                name: "KeycloakData",
                table: "tb_oneid_account_saga_state",
                newName: "Payload");
        }
    }
}
