using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComposeExample.Api.Data.Design
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MoviesContext>
    {
        public MoviesContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MoviesContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MoviesMigrations;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new MoviesContext(optionsBuilder.Options);
        }
    }
}
