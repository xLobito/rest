using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class CursoContext : DbContext
    {
        public CursoContext (DbContextOptions<CursoContext> options) : base(options)
        {
            ;
        }

        public DbSet<Curso> Cursos { get; set; }
    }
}