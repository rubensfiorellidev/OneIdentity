using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFullnameAndLoginToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar", maxLength: 26, nullable: false),
                    fullname = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    login = table.Column<string>(type: "varchar", maxLength: 150, nullable: false),
                    provisioning_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_oneid_role_claims_tb_oneid_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tb_oneid_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "varchar", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_claims_tb_oneid_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_oneid_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "varchar", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_logins_tb_oneid_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_oneid_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_user_roles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_roles_tb_oneid_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tb_oneid_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_roles_tb_oneid_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_oneid_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_user_tokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_tokens_tb_oneid_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_oneid_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_role_claims_RoleId",
                table: "tb_oneid_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_roles_normalized_name",
                table: "tb_oneid_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_logins_UserId",
                table: "tb_oneid_user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_roles_RoleId",
                table: "tb_oneid_user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_users_normalized_email",
                table: "tb_oneid_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_users_normalized_user_name",
                table: "tb_oneid_users",
                column: "normalized_user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_role_claims");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_claims");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_logins");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_roles");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_tokens");

            migrationBuilder.DropTable(
                name: "tb_oneid_roles");

            migrationBuilder.DropTable(
                name: "tb_oneid_users");
        }
    }
}
