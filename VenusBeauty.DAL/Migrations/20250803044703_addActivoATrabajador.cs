using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenusBeauty.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addActivoATrabajador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Trabajadores",
                type: "bit",
                nullable: false,
                defaultValue: true);

      
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
        }
    }
}
