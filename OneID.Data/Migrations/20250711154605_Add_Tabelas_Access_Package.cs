using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tabelas_Access_Package : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_access_package",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProvisioningAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_access_package", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_access_package_condition",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    AccessPackageId = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_access_package_condition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_oneid_access_package_condition_tb_oneid_access_package_A~",
                        column: x => x.AccessPackageId,
                        principalTable: "tb_oneid_access_package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_access_package_items",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    AccessPackageId = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_access_package_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_oneid_access_package_items_tb_oneid_access_package_Acces~",
                        column: x => x.AccessPackageId,
                        principalTable: "tb_oneid_access_package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_access_package_condition_AccessPackageId",
                table: "tb_oneid_access_package_condition",
                column: "AccessPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_oneid_access_package_items_AccessPackageId",
                table: "tb_oneid_access_package_items",
                column: "AccessPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_access_package_condition");

            migrationBuilder.DropTable(
                name: "tb_oneid_access_package_items");

            migrationBuilder.DropTable(
                name: "tb_oneid_access_package");
        }
    }
}
