using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenusBeauty.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddImageURLaProductosyServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
   

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Trabajadores",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Servicios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Producto",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_UserId",
                table: "Trabajadores",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
  

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Producto");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Trabajadores",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_UserId",
                table: "Trabajadores",
                column: "UserId");
        }
    }
}
