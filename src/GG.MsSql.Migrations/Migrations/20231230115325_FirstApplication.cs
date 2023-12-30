using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GG.Migrations.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class FirstApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailHosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Host = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    UseSsl = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailHosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailHostUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailHostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailHostUsers", x => new { x.EmailHostId, x.UserId });
                    table.ForeignKey(
                        name: "FK_EmailHostUsers_EmailHosts_EmailHostId",
                        column: x => x.EmailHostId,
                        principalTable: "EmailHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_.EmailHostUser_.EmailHostId",
                table: "EmailHostUsers",
                column: "EmailHostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailHostUsers");

            migrationBuilder.DropTable(
                name: "EmailHosts");
        }
    }
}
