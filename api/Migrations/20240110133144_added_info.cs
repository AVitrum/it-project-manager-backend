using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class added_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalUserInfos",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalUserInfos", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AdditionalUserInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalUserInfos");
        }
    }
}
