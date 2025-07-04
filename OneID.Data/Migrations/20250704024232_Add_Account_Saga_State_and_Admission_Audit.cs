using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Account_Saga_State_and_Admission_Audit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_oneid_account_saga_state",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    fault_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_account_saga_state", x => x.correlation_id);
                });

            migrationBuilder.CreateTable(
                name: "tb_oneid_automatic_admission_audit",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    firstname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    lastname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    database_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    current_state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    event_name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    provisioning_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_oneid_automatic_admission_audit", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_oneid_account_saga_state");

            migrationBuilder.DropTable(
                name: "tb_oneid_automatic_admission_audit");
        }
    }
}
