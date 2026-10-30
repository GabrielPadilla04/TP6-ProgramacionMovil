using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP6_Servicios_API_REST.Migrations
{
    /// <inheritdoc />
    public partial class LimpiarUsuarioIdDeSalidas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salidas_Clientes_ClienteId",
                table: "Salidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Salidas_Usuarios_UsuarioId",
                table: "Salidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Salidas_SalidaId",
                table: "Transacciones");

            migrationBuilder.DropIndex(
                name: "IX_Salidas_UsuarioId",
                table: "Salidas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Salidas");

            migrationBuilder.AddForeignKey(
                name: "FK_Salidas_Usuarios_ClienteId",
                table: "Salidas",
                column: "ClienteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Salidas_SalidaId",
                table: "Transacciones",
                column: "SalidaId",
                principalTable: "Salidas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salidas_Usuarios_ClienteId",
                table: "Salidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Salidas_SalidaId",
                table: "Transacciones");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Salidas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Salidas_UsuarioId",
                table: "Salidas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Salidas_Clientes_ClienteId",
                table: "Salidas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Salidas_Usuarios_UsuarioId",
                table: "Salidas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Salidas_SalidaId",
                table: "Transacciones",
                column: "SalidaId",
                principalTable: "Salidas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
