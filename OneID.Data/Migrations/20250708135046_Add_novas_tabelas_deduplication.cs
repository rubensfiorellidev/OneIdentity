using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_novas_tabelas_deduplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_account_admission_staging",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SocialName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    CpfHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FiscalNumberIdentity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiscalNumberIdentityHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ContractorCnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    ContractorCnpjHash = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    ContractorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    JobTitleId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoginHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PersonalEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PersonalEmailHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CorporateEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CorporateEmailHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_account_admission_staging", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_deduplication",
                columns: table => new
                {
                    CorrelationId = table.Column<string>(type: "VARCHAR", maxLength: 300, nullable: false),
                    ProcessName = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_deduplication", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_deduplication_key",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessKey = table.Column<string>(type: "VARCHAR", maxLength: 300, nullable: false),
                    ProcessName = table.Column<string>(type: "VARCHAR", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_deduplication_key", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_admission_staging_correlation",
                table: "tb_oneid_account_admission_staging",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_admission_staging_cpfhash",
                table: "tb_oneid_account_admission_staging",
                column: "CpfHash");

            migrationBuilder.CreateIndex(
                name: "idx_admission_staging_loginhash",
                table: "tb_oneid_account_admission_staging",
                column: "LoginHash");

            migrationBuilder.CreateIndex(
                name: "ux_deduplication_business_process",
                table: "tb_oneid_deduplication",
                columns: new[] { "CorrelationId", "ProcessName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_deduplication_key_business_process",
                table: "tb_oneid_deduplication_key",
                columns: new[] { "BusinessKey", "ProcessName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_account_admission_staging");

            migrationBuilder.DropTable(
                name: "tb_oneid_deduplication");

            migrationBuilder.DropTable(
                name: "tb_oneid_deduplication_key");
        }
    }
}
