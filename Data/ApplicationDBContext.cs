using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order_Genie.Models;

namespace Order_Genie.Data
{
    public class ApplicationDBContext : DbContext
    {
public virtual DbSet<User> Users { get; set; }
        
        protected virtual void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("server=127.0.0.1;port=3306;user=root;password=root;database=ordergenie")
                .UseLoggerFactory(LoggerFactory.Create(b => b
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Information)))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            
        }
    }
}
