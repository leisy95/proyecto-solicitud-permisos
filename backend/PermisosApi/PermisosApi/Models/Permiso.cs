using System.ComponentModel.DataAnnotations;

namespace PermisosApi.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Correo { get; set; }
        
        [Required(ErrorMessage = "El motivo es obligatorio")]
        public string Motivo { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string? ArchivoPdf { get; set; }
    }
}
