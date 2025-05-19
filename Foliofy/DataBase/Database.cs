using Microsoft.EntityFrameworkCore;
using Foliofy.Models;

namespace Foliofy.DataBase
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
    }
}
