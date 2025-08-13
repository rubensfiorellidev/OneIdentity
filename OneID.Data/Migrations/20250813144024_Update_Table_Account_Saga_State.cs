using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneID.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Account_Saga_State : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "tb_oneid_account_saga_state",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FaultReason",
                table: "tb_oneid_account_saga_state",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DatabaseId",
                table: "tb_oneid_account_saga_state",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentState",
                table: "tb_oneid_account_saga_state",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "AzureCreated",
                table: "tb_oneid_account_saga_state",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AzureUserId",
                table: "tb_oneid_account_saga_state",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorporateEmail",
                table: "tb_oneid_account_saga_state",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DatabaseCreated",
                table: "tb_oneid_account_saga_state",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KeycloakCreated",
                table: "tb_oneid_account_saga_state",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KeycloakUserId",
                table: "tb_oneid_account_saga_state",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastEvent",
                table: "tb_oneid_account_saga_state",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LoginAllocated",
                table: "tb_oneid_account_saga_state",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "tb_oneid_account_saga_state",
                type: "timestamptz",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzureCreated",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "AzureUserId",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "CorporateEmail",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "DatabaseCreated",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "KeycloakCreated",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "KeycloakUserId",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "LastEvent",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "LoginAllocated",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "tb_oneid_account_saga_state");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "tb_oneid_account_saga_state",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FaultReason",
                table: "tb_oneid_account_saga_state",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DatabaseId",
                table: "tb_oneid_account_saga_state",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentState",
                table: "tb_oneid_account_saga_state",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }
    }
}
