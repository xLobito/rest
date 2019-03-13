using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class NotaContext : DbContext
    {
        public NotaContext (DbContextOptions<NotaContext> options) : base(options)
        {
            ;
        }

        public DbSet<Nota> Notas { get; set; }
    }
}