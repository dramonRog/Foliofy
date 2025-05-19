using Microsoft.EntityFrameworkCore;
using Foliofy.Models;

namespace Foliofy.DataBase
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
