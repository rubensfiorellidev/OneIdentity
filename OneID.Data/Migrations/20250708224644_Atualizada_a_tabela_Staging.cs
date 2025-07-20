using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Atualizada_a_tabela_Staging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatabaseId",
                table: "tb_oneid_account_saga_state",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "tb_oneid_account_saga_state",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "tb_oneid_account_admission_staging",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginManager",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Registry",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusUserAccount",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeUserAccount",
                table: "tb_oneid_account_admission_staging",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseId",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "LoginManager",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "Registry",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "StatusUserAccount",
                table: "tb_oneid_account_admission_staging");

            migrationBuilder.DropColumn(
                name: "TypeUserAccount",
                table: "tb_oneid_account_admission_staging");
        }
    }
}
