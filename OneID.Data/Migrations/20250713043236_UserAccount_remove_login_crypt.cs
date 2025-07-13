using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserAccount_remove_login_crypt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginCrypt",
                table: "tb_oneid_user_accounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginCrypt",
                table: "tb_oneid_user_accounts",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
