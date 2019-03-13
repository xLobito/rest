using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class CarreraContext : DbContext
    {
        public CarreraContext (DbContextOptions<CarreraContext> options) : base(options)
        {
            ;
        }

        public DbSet<Carrera> Carreras{ get; set; }
    }
}