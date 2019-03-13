using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class ModuloContext : DbContext
    {
        public ModuloContext (DbContextOptions<ModuloContext> options) : base(options)
        {
            ;
        }

        public DbSet<Modulo> Modulos { get; set; }
    }
}