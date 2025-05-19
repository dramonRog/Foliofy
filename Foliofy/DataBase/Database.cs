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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Follower>()
                .HasOne(follower => follower.FollowerUser)
                .WithMany(user => user.Followings)
                .HasForeignKey(follower => follower.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follower>()
                .HasOne(follower => follower.FollowedUser)
                .WithMany(user => user.Followers)
                .HasForeignKey(follower => follower.FollowedId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
