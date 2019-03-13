using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class IngresoContext : DbContext
    {
        public IngresoContext (DbContextOptions<IngresoContext> options) : base(options)
        {
            ;
        }

        public DbSet<Ingreso> Ingresos{ get; set; }
    }
}