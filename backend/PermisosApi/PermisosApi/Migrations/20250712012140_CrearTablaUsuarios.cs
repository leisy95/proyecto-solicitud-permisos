using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermisosApi.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Usuarios",
                newName: "Rol");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Usuarios",
                newName: "Nombre");

            migrationBuilder.AddColumn<string>(
                name: "ContraseñaHash",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContraseñaHash",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Usuarios",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Usuarios",
                newName: "Password");
        }
    }
}
