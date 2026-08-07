using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGimn.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoUrlCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Clientes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Clientes");
        }
    }
}