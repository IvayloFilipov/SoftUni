namespace MoiteRecepti.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class OriginalUrlRecipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "Recipes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "Recipes");
        }
    }
}
