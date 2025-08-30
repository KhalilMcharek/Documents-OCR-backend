using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documents_OCR_back.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestionsJsonProperly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuggestionsJson",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuggestionsJson",
                table: "Documents");
        }
    }
}
