using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=127.0.0.1,1433;Database=NulpDb;User Id=SA;Password=Your_password123;Trusted_Connection=True;Integrated Security=False");
            optionsBuilder.UseSqlServer(@"Server=tcp:testnulpserver.database.windows.net,1433;Initial Catalog=NulpEntity;User ID=nulp;Password=Venherets.1996;MultipleActiveResultSets=False;Connection Timeout=30;");
            
        }
    }
}
