using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP6_Servicios_API_REST.Migrations
{
    /// <inheritdoc />
    public partial class RelacionProveedorUsuarioProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Proveedores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_UsuarioId",
                table: "Proveedores",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Usuarios_UsuarioId",
                table: "Proveedores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Usuarios_UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Productos");
        }
    }
}
