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
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowerUser)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowedUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTag>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_UserTag_Association",
                "([UserId] IS NOT NULL AND [ProjectId] IS NULL) OR ([UserId] IS NULL AND [ProjectId] IS NOT NULL)"
            ));
        }
    }
}
