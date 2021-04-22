using Microsoft.EntityFrameworkCore.Migrations;

namespace P03_FootballBetting.Data.Migrations
{
    public partial class PlayerStatisticMinutesPlayIsInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinutesPlayed",
                table: "PlayerStatistics",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Prediction",
                table: "Bets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prediction",
                table: "Bets");

            migrationBuilder.AlterColumn<double>(
                name: "MinutesPlayed",
                table: "PlayerStatistics",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
