using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tabela_Usuario_de_Servico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_service_user",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_service_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_service_user_claims",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServiceUserId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_service_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_oneid_service_user_claims_tb_oneid_service_user_ServiceU~",
                        column: x => x.ServiceUserId,
                        principalTable: "tb_oneid_service_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_service_user_claims_ServiceUserId",
                table: "tb_oneid_service_user_claims",
                column: "ServiceUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_service_user_claims");

            migrationBuilder.DropTable(
                name: "tb_oneid_service_user");
        }
    }
}
