using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaChallengeAndTotpReplay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_mfa_challenge",
                columns: table => new
                {
                    Jti = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CodeChallenge = table.Column<string>(type: "text", nullable: false),
                    IpHash = table.Column<string>(type: "text", nullable: false),
                    UserAgentHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    ConsumedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_mfa_challenge", x => x.Jti);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_totp_code_used_window",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Step = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_totp_code_used_window", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_mfa_challenge_active_by_expiry",
                table: "tb_oneid_mfa_challenge",
                column: "ExpiresAt",
                filter: "\"Used\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_mfa_challenge_used",
                table: "tb_oneid_mfa_challenge",
                column: "Used");

            migrationBuilder.CreateIndex(
                name: "ix_mfa_challenge_user_id",
                table: "tb_oneid_mfa_challenge",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_mfa_challenge_user_used_expires",
                table: "tb_oneid_mfa_challenge",
                columns: new[] { "UserId", "Used", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_mfa_challenge_UserId_CodeChallenge",
                table: "tb_oneid_mfa_challenge",
                columns: new[] { "UserId", "CodeChallenge" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_totp_code_use_expires_at",
                table: "tb_oneid_totp_code_used_window",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "ix_totp_code_use_user_step",
                table: "tb_oneid_totp_code_used_window",
                columns: new[] { "UserId", "Step" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_mfa_challenge");

            migrationBuilder.DropTable(
                name: "tb_oneid_totp_code_used_window");
        }
    }
}
