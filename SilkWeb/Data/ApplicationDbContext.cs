using Microsoft.EntityFrameworkCore;
using SilkWeb.Models;

namespace SilkWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base (options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }
    }
}
