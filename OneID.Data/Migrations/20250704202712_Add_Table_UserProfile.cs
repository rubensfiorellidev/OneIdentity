using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_UserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_user_profiles",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    social_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    date_of_hire = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    date_of_fired = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    registry = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mother_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    company = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    login_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    corporate_email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    corporate_email_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    personal_email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    personal_email_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status_user_profile = table.Column<int>(type: "integer", nullable: false),
                    type_user_profile = table.Column<int>(type: "integer", nullable: false),
                    is_inactive = table.Column<bool>(type: "boolean", nullable: false),
                    login_manager = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    position_held_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fiscal_number_identity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    contractor_cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    contractor_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    provisioning_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_profiles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_user_profile_corporate_email",
                table: "tb_oneid_user_profiles",
                column: "corporate_email");

            migrationBuilder.CreateIndex(
                name: "idx_user_profile_cpf",
                table: "tb_oneid_user_profiles",
                column: "cpf");

            migrationBuilder.CreateIndex(
                name: "idx_user_profile_login",
                table: "tb_oneid_user_profiles",
                column: "login");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_user_profiles");
        }
    }
}
