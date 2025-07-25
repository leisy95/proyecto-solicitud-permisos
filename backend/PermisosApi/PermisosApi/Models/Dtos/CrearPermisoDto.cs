using System.ComponentModel.DataAnnotations;

namespace PermisosApi.Models.Dtos
{
    public class CrearPermisoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        public string Motivo { get; set; }

        public IFormFile? Archivo { get; set; }
    }
}
