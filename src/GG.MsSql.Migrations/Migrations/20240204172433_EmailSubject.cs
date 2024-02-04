﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GG.Migrations.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class EmailSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "EmailTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "EmailTemplates");
        }
    }
}
