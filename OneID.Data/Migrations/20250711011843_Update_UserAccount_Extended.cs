using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserAccount_Extended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_users_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_users_UserId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropTable(
                name: "tb_oneid_role_claims");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_logins");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_tokens");

            migrationBuilder.DropTable(
                name: "tb_oneid_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_oneid_user_roles",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropIndex(
                name: "ix_tb_oneid_roles_normalized_name",
                table: "tb_oneid_roles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropColumn(
                name: "ClaimType",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "ClaimValue",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "tb_oneid_roles");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "tb_oneid_roles");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "tb_oneid_user_roles",
                type: "character varying(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "tb_oneid_user_roles",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");


            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "tb_oneid_user_claims",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAccountId",
                table: "tb_oneid_user_claims",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "tb_oneid_user_claims",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalEmailHash",
                table: "tb_oneid_user_accounts",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastLoginAt",
                table: "tb_oneid_user_accounts",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginCrypt",
                table: "tb_oneid_user_accounts",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "tb_oneid_user_accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "tb_oneid_roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tb_oneid_roles",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "tb_oneid_roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_oneid_user_roles",
                table: "tb_oneid_user_roles",
                columns: new[] { "UserAccountId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_claims_UserAccountId",
                table: "tb_oneid_user_claims",
                column: "UserAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_claims",
                column: "UserAccountId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_roles",
                column: "UserAccountId",
                principalTable: "tb_oneid_user_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_user_accounts_UserAccountId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tb_oneid_user_roles",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropIndex(
                name: "IX_tb_oneid_user_claims_UserAccountId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "tb_oneid_user_roles");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "tb_oneid_user_claims");

            migrationBuilder.DropColumn(
                name: "KeycloakUserId",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropColumn(
                name: "LoginCrypt",
                table: "tb_oneid_user_accounts");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "tb_oneid_user_accounts");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "tb_oneid_user_roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tb_oneid_user_roles",
                type: "varchar",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "tb_oneid_user_claims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ClaimType",
                table: "tb_oneid_user_claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimValue",
                table: "tb_oneid_user_claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tb_oneid_user_claims",
                type: "varchar",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalEmailHash",
                table: "tb_oneid_user_accounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "tb_oneid_roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tb_oneid_roles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "tb_oneid_roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "tb_oneid_roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "tb_oneid_roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tb_oneid_user_roles",
                table: "tb_oneid_user_roles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateTable(
                name: "tb_oneid_role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                name: "tb_oneid_users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar", maxLength: 26, nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    KeycloakUserId = table.Column<string>(type: "varchar", maxLength: 100, nullable: true),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LoginCrypt = table.Column<string>(type: "varchar", maxLength: 250, nullable: false),
                    LoginHash = table.Column<string>(type: "varchar", maxLength: 150, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ProvisioningAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_users", x => x.Id);
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
                name: "IX_tb_oneid_user_claims_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_roles_normalized_name",
                table: "tb_oneid_roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_role_claims_RoleId",
                table: "tb_oneid_role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_user_logins_UserId",
                table: "tb_oneid_user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_users_normalized_email",
                table: "tb_oneid_users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "ix_tb_oneid_users_normalized_user_name",
                table: "tb_oneid_users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_claims_tb_oneid_users_UserId",
                table: "tb_oneid_user_claims",
                column: "UserId",
                principalTable: "tb_oneid_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_oneid_user_roles_tb_oneid_users_UserId",
                table: "tb_oneid_user_roles",
                column: "UserId",
                principalTable: "tb_oneid_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
