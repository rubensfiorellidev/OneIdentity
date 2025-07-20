using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Stored_Events_AND_Users_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_stored_events",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EventData = table.Column<string>(type: "jsonb", nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_stored_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_user_accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProvisioningAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SocialName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    CpfHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    DateOfHire = table.Column<DateTime>(type: "date", nullable: false),
                    DateOfFired = table.Column<DateTime>(type: "date", nullable: true),
                    Registry = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MotherName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Company = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoginHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CorporateEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CorporateEmailHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PersonalEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PersonalEmailHash = table.Column<string>(type: "text", nullable: true),
                    StatusUserAccount = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    TypeUserAccount = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    IsInactive = table.Column<bool>(type: "boolean", nullable: false),
                    LoginManager = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    JobTitleId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FiscalNumberIdentity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiscalNumberIdentityHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContractorCnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    ContractorCnpjHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContractorName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_user_accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_user_account_corporate_email",
                table: "tb_oneid_user_accounts",
                column: "CorporateEmail");

            migrationBuilder.CreateIndex(
                name: "idx_user_account_cpf",
                table: "tb_oneid_user_accounts",
                column: "Cpf");

            migrationBuilder.CreateIndex(
                name: "idx_user_account_login",
                table: "tb_oneid_user_accounts",
                column: "Login");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_stored_events");

            migrationBuilder.DropTable(
                name: "tb_oneid_user_accounts");
        }
    }
}
