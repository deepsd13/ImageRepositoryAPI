using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageRepository.Migrations
{
    public partial class ChangedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "size",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Images",
                newName: "ImageTitle");

            migrationBuilder.RenameColumn(
                name: "imageData",
                table: "Images",
                newName: "ImageName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageTitle",
                table: "Images",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Images",
                newName: "imageData");

            migrationBuilder.AddColumn<int>(
                name: "size",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
