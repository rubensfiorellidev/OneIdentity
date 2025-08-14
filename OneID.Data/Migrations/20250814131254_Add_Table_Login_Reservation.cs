using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_Login_Reservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_idempotency",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_idempotency", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_login_reservations",
                columns: table => new
                {
                    Login = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_login_reservations", x => x.Login);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_login_reservations_CorrelationId",
                table: "tb_oneid_login_reservations",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_login_reservations_Status",
                table: "tb_oneid_login_reservations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_idempotency");

            migrationBuilder.DropTable(
                name: "tb_oneid_login_reservations");
        }
    }
}
