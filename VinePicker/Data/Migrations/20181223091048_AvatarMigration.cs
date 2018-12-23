using Microsoft.EntityFrameworkCore.Migrations;

namespace VinePicker.Data.Migrations
{
    public partial class AvatarMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarStr",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarStr",
                table: "AspNetUsers");
        }
    }
}
