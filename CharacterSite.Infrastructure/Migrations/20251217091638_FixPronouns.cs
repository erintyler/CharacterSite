using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharacterSite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPronouns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Object",
                table: "Pronouns",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Possessive",
                table: "Pronouns",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Pronouns",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Object",
                table: "Pronouns");

            migrationBuilder.DropColumn(
                name: "Possessive",
                table: "Pronouns");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Pronouns");
        }
    }
}
