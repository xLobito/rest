using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext (DbContextOptions<UsuarioContext> options) : base(options)
        {
            ;
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}