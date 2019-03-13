using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class BeneficioContext : DbContext
    {
        public BeneficioContext (DbContextOptions<BeneficioContext> options) : base(options)
        {
            ;
        }

        public DbSet<Beneficio> Beneficios { get; set; }
    }
}