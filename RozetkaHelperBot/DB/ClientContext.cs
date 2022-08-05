using Microsoft.EntityFrameworkCore;

namespace RozetkaHelperBot.DB
{
    class ClientContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ClientsDBConnection;Trusted_Connection=True;");
        }
    }
}
