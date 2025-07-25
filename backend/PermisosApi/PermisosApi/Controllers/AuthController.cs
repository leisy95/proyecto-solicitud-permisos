using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PermisosApi.Data;
using PermisosApi.Models;
using PermisosApi.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;
using PermisosApi.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public AuthController(ApplicationDbContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (user == null)
                return Unauthorized(new { mensaje = "Correo no registrado" });

            if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, user.ContrasenaHash))
                return Unauthorized(new { mensaje = "Contraseña incorrecta" });

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.Correo),
            new Claim(ClaimTypes.Role, user.Rol)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                rol = user.Rol
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR EN LOGIN");
            Console.WriteLine("Mensaje: " + ex.Message);
            Console.WriteLine("Interno: " + ex.InnerException?.Message);
            return StatusCode(500, new { mensaje = "Error interno", error = ex.Message });
        }
    }

    [HttpPost("crear-usuario")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegistrarUsuarioDto dto)
    {
        var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
        if (existe)
            return BadRequest(new { mensaje = "El correo ya está registrado" });

        var contrasenaGenerada = GenerarContrasena();
        var hash = BCrypt.Net.BCrypt.HashPassword(contrasenaGenerada);

        var nuevoUsuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            ContrasenaHash = hash,
            Rol = dto.Rol
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        // Enviar correo
        await _emailService.EnviarCorreoAsync(dto.Correo, "Tu acceso al sistema",
            $"Hola {dto.Nombre}, tu contraseña es: {contrasenaGenerada}");

        return Ok(new { mensaje = "Usuario creado y correo enviado" });
    }

    private string GenerarContrasena()
    {
        var caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#!$";
        return new string(Enumerable.Repeat(caracteres, 10)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    //[HttpPost("registrar")]
    //public async Task<IActionResult> RegistrarTemporal()
    //{
    //    var usuario = new Usuario
    //    {
    //        Nombre = "Admin",
    //        Correo = "tucorreo@ejemplo.com",
    //        ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
    //        Rol = "Admin"
    //    };

    //    _context.Usuarios.Add(usuario);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { mensaje = "Usuario creado para prueba." });
    //}
}
