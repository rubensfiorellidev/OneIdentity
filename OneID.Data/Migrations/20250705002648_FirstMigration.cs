using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_account_saga_state",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    FaultReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_account_saga_state", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_automatic_admission_audit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Firstname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Lastname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DatabaseId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ProvisioningDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_automatic_admission_audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar", maxLength: 26, nullable: false),
                    Fullname = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    FirstName = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    LoginHash = table.Column<string>(type: "varchar", maxLength: 150, nullable: false),
                    LoginCrypt = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    ProvisioningAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar", maxLength: 100, nullable: true),
                    KeycloakUserId = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_users", x => x.Id);
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
                        principalColumn: "Id",
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
                        principalColumn: "Id",
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
                        principalColumn: "Id",
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_oneid_user_roles_tb_oneid_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_oneid_users",
                        principalColumn: "Id",
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_role_claims_RoleId",
                table: "tb_oneid_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_roles_normalized_name",
                table: "tb_oneid_roles",
                column: "NormalizedName",
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
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_users_normalized_user_name",
                table: "tb_oneid_users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_account_saga_state");

            migrationBuilder.DropTable(
                name: "tb_oneid_automatic_admission_audit");

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
