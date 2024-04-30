using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class added_roles_to_employee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PositionInCompanyId",
                table: "UserCompanies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_PositionInCompanyId",
                table: "UserCompanies",
                column: "PositionInCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_PositionInCompanies_PositionInCompanyId",
                table: "UserCompanies",
                column: "PositionInCompanyId",
                principalTable: "PositionInCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_PositionInCompanies_PositionInCompanyId",
                table: "UserCompanies");

            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_PositionInCompanyId",
                table: "UserCompanies");

            migrationBuilder.DropColumn(
                name: "PositionInCompanyId",
                table: "UserCompanies");
        }
    }
}
