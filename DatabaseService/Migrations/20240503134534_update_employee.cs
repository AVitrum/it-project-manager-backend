using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseService.Migrations
{
    /// <inheritdoc />
    public partial class update_employee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PositionInCompanies",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "Priority",
                table: "PositionInCompanies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "PositionInCompanies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PositionInCompanies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
