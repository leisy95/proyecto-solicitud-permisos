using System.ComponentModel.DataAnnotations;

namespace PermisosApi.Models.Dtos
{
    public class RegistrarUsuarioDto
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        //[Required]
        //public string Contrasena { get; set; }

        public string Rol { get; set; } = "Usuario";
    }
}
