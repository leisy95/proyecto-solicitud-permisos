namespace PermisosApi.Models.Dtos
{
    public class PermisoRespuestaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
        public string ArchivoPdf { get; set; }
    }
}
