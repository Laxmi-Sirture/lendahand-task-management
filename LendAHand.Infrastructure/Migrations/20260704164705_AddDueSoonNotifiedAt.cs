using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendAHand.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDueSoonNotifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueSoonNotifiedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueSoonNotifiedAt",
                table: "Tasks");
        }
    }
}
