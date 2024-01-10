using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class updated_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalUserInfos",
                table: "AdditionalUserInfos");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AdditionalUserInfos",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalUserInfos",
                table: "AdditionalUserInfos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalUserInfos_UserId",
                table: "AdditionalUserInfos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalUserInfos",
                table: "AdditionalUserInfos");

            migrationBuilder.DropIndex(
                name: "IX_AdditionalUserInfos_UserId",
                table: "AdditionalUserInfos");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AdditionalUserInfos",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalUserInfos",
                table: "AdditionalUserInfos",
                column: "UserId");
        }
    }
}
