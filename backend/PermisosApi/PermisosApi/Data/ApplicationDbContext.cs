using Microsoft.EntityFrameworkCore;
using PermisosApi.Models;
using System.Collections.Generic;

namespace PermisosApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
