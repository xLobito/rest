using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class AvisoContext : DbContext
    {
        public AvisoContext (DbContextOptions<AvisoContext> options) : base(options)
        {
            ;
        }

        public DbSet<Aviso> Avisos { get; set; }
    }
}