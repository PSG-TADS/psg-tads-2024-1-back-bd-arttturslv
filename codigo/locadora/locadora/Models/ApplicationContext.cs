using Microsoft.EntityFrameworkCore;


namespace locadora.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Locacao> Locacao { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }

    }
}
