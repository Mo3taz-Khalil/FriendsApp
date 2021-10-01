using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        
        public DataContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>()
                .HasKey(k=>new {k.SourceUserId,k.LikedUserId});
            builder.Entity<UserLike>()
                .HasOne(s=>s.SourceUser)
                .WithMany(S=>S.LikedByMe)
                .HasForeignKey(K=>K.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>()
                .HasOne(s=>s.LikedUser)
                .WithMany(S=>S.LikedByUsers)
                .HasForeignKey(K=>K.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}