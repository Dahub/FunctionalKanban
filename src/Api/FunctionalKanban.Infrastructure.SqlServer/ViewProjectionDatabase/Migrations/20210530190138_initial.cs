namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "DeletedTasks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueIdentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueIdentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalRemaningWork = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueIdentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueIdentifier", nullable: true),
                    RemaningWork = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedTasks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "dbo");
        }
    }
}
