using Microsoft.EntityFrameworkCore.Migrations;

namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase.Migrations
{
    public partial class add_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_EntityId",
                schema: "dbo",
                table: "Events",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EntityId_EntityName_Version",
                schema: "dbo",
                table: "Events",
                columns: new[] { "EntityId", "EntityName", "Version" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_EntityId",
                schema: "dbo",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_EntityId_EntityName_Version",
                schema: "dbo",
                table: "Events");
        }
    }
}
