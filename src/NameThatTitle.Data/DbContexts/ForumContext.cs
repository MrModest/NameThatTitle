using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NameThatTitle.Domain.Models.Forum;
using NameThatTitle.Domain.Models.Users;
using NameThatTitle.Domain.Static;

namespace NameThatTitle.Data.DbContexts
{
    public class ForumContext : DbContext
    {
        public ForumContext(DbContextOptions<ForumContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserStatistic> UserStatistics { get; set; }

        public DbSet<PrivateMessage> PrivateMessages { get; set; }

        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        //public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserProfile>(ConfigureUserProfile);
            builder.Entity<UserStatistic>(ConfigureUserStatistic);

            builder.Entity<Forum>(ConfigureForum);
            builder.Entity<Post>(ConfigurePost);
            builder.Entity<Comment>(ConfigureComment);
            //builder.Entity<Attachment>(ConfigureAttachment);
            builder.Entity<Subscription>(ConfigureSubscription);
            builder.Entity<Favorite>(ConfigureFavorite);
        }

        private void ConfigureUserProfile(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(up => up.Id);

            builder.HasOne(up => up.Statistic)
                .WithOne(us => us.Profile)
                .HasForeignKey<UserStatistic>(us => us.Id);

            builder.HasMany(up => up.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            builder.HasMany(up => up.Favorites)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);


            builder.Ignore(up => up.Account);
        }

        private void ConfigureUserStatistic(EntityTypeBuilder<UserStatistic> builder)
        {
            builder.HasKey(us => us.Id);
        }

        private void ConfigureForum(EntityTypeBuilder<Forum> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .HasMaxLength(StaticData.Forum.Name.MaxLength)
                .IsRequired();

            //ToDo: Pagination!
            builder.HasMany(f => f.Posts)
                .WithOne(p => p.Forum)
                .HasForeignKey(p => p.ForumId);
        }

        private void ConfigurePost(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ForumId)
                .IsRequired();

            builder.Property(p => p.AuthorId)
                .IsRequired();

            builder.Property(p => p.Title)
                .HasMaxLength(StaticData.Post.Title.MaxLength)
                .IsRequired();


            builder.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId);

            //ToDo: Pagination!
            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);

            builder.HasMany(p => p.Subscriptions)
                .WithOne(s => s.Post)
                .HasForeignKey(s => s.PostId);

            builder.HasMany(p => p.Favorites)
                .WithOne(f => f.Post)
                .HasForeignKey(f => f.PostId);
        }

        private void ConfigureComment(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.PostId)
                .IsRequired();

            builder.Property(c => c.AuthorId)
                .IsRequired();


            //? It's work? - https://stackoverflow.com/questions/29516342
            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Childs)
                .HasForeignKey(c => c.ParentId);

            builder.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId);
        }

        private void ConfigureSubscription(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => new
            {
                s.UserId,
                s.PostId
            });
        }

        private void ConfigureFavorite(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => new
            {
                f.UserId,
                f.PostId
            });
        }
    }
}