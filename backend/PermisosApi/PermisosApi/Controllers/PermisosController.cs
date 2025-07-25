using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermisosApi.Data;
using PermisosApi.Models;
using PermisosApi.Models.Dtos;
using PermisosApi.Services;

namespace PermisosApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PermisosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public PermisosController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // POST: Crear una solicitud
        [HttpPost]
        public async Task<IActionResult> SolicitarPermiso([FromForm] CrearPermisoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(errores);
                }

                // Validación extra del archivo (opcional)
                if (dto.Archivo != null && dto.Archivo.Length > 0)
                {
                    var nombreArchivo = Path.GetFileName(dto.Archivo.FileName);
                    var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Archivos", nombreArchivo);

                    using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                    {
                        await dto.Archivo.CopyToAsync(stream);
                    }
                }

                // Guardar en la base de datos
                var permiso = new Permiso
                {
                    Nombre = dto.Nombre,
                    Correo = dto.Correo,
                    Motivo = dto.Motivo,
                    FechaSolicitud = DateTime.Now,
                    Estado = "Pendiente",
                    ArchivoPdf = dto.Archivo?.FileName // Esto es un campo nuevo en tu entidad Permiso
                };

                _context.Permisos.Add(permiso);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Solicitud guardada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar solicitud: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}"); // <-- ESTA LÍNEA
                return StatusCode(500, new { mensaje = "Error interno", error = ex.Message });
            }
        }

        // GET: Obtener todas las solicitudes 
        [Authorize]
        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudes([FromQuery] string? fecha, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            Console.WriteLine($"Fecha recibida: {fecha}");
            var query = _context.Permisos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (DateTime.TryParse(fecha, out DateTime fechaParseada))
                {
                    var siguienteDia = fechaParseada.Date.AddDays(1);
                    query = query.Where(p => p.FechaSolicitud >= fechaParseada.Date && p.FechaSolicitud < siguienteDia);
                }
                else
                {
                    return BadRequest(new { mensaje = "Formato de fecha inválido." });
                }
            }

            var total = await query.CountAsync();

            var solicitudes = await query
                .OrderByDescending(p => p.FechaSolicitud)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermisoRespuestaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Correo = p.Correo,
                    Motivo = p.Motivo,
                    FechaSolicitud = p.FechaSolicitud,
                    Estado = p.Estado,

                    ArchivoPdf = p.ArchivoPdf
                }).ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = solicitudes
            });
        }

        // PUT: Actualizar estado
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] EstadoPermisoDto dto)
        {
            try
            {
                var permiso = await _context.Permisos.FindAsync(id);
                if (permiso == null)
                    return NotFound();

                if (string.IsNullOrWhiteSpace(dto.Estado))
                    return BadRequest(new { mensaje = "El estado es requerido" });

                permiso.Estado = dto.Estado;
                await _context.SaveChangesAsync();

                // Enviar correo (de forma segura)
                var asunto = "Estado de tu solicitud de permiso";
                var cuerpo = $"Hola {permiso.Nombre},\n\nTu solicitud para {permiso.Motivo} ha sido {dto.Estado.ToUpper()}.";

                try
                {
                    await _emailService.EnviarCorreoAsync(permiso.Correo, asunto, cuerpo);
                }
                catch (Exception correoEx)
                {
                    Console.WriteLine($"❌ Error al enviar correo: {correoEx.Message}");
                }

                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR al actualizar estado:");
                Console.WriteLine($"DTO recibido: {dto.Estado}");
                Console.WriteLine($"Permiso ID: {id}");
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al actualizar", error = ex.Message });
            }
        }

    }
}