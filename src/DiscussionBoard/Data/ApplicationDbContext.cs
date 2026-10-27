using DiscussionBoard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DiscussionThread>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.DiscussionThread)
                .HasForeignKey(t => t.DiscussionThreadId);
            builder.Entity<Post>(entity =>
            {
                entity.HasOne(p => p.DiscussionThread)
                    .WithMany(t => t.Posts)
                    .HasForeignKey(i => i.DiscussionThreadId);
                entity.HasMany(t => t.Replies)
                    .WithOne(p => p.ParentPost)
                    .HasForeignKey(i => i.ParentPostId)
                    .IsRequired(false);
            });
            builder.Entity<Forum>()
                .HasMany(p => p.DiscussionThreads)
                .WithOne(t => t.Forum)
                .HasForeignKey(t => t.ForumId);
        }
    }
}
