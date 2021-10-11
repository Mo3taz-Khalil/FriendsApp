using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {

        public DataContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(k => k.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                 .HasMany(u => u.UserRoles)
                 .WithOne(u => u.Role)
                 .HasForeignKey(k => k.RoleId)
                 .IsRequired();

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(S => S.LikedByMe)
                .HasForeignKey(K => K.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(S => S.LikedByUsers)
                .HasForeignKey(K => K.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(m => m.MessagesSent)
                .HasForeignKey(k => k.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(m => m.MessagesReceived)
                .HasForeignKey(k => k.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}