namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase.Migrations
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
                name: "Events",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueIdentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueIdentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Datas = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events",
                schema: "dbo");
        }
    }
}
