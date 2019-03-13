using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class BecadoContext : DbContext
    {
        public BecadoContext (DbContextOptions<BecadoContext> options) : base(options)
        {
            ;
        }

        public DbSet<Becado> Becados { get; set; }
    }
}