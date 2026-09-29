using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTrade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicateRoleNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");
        }
    }
}
