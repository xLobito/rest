using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class InscritoContext : DbContext
    {
        public InscritoContext (DbContextOptions<InscritoContext> options) : base(options)
        {
            ;
        }

        public DbSet<Inscrito> Inscritos { get; set; }
    }
}