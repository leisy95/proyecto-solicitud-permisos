using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PermisosApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [Column("ContraseñaHash")]
        public string ContrasenaHash { get; set; }

        [Required]
        public string Rol { get; set; } = "Usuario"; //Para usuario o administrador    
    }
}
