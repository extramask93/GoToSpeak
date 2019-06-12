using Microsoft.EntityFrameworkCore.Migrations;

namespace GoToSpeak.Migrations
{
    public partial class addedPublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPublicID",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPublicID",
                table: "Users");
        }
    }
}
