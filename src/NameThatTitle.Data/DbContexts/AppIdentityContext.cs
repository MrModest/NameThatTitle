using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using NameThatTitle.Core.Models.Users;
using NameThatTitle.Core.Models.Forum;
using NameThatTitle.Core.Static;
using NameThatTitle.Core.Models.Token;

namespace NameThatTitle.Data.DbContexts
{
    public class AppIdentityContext : IdentityDbContext<UserAccount, UserRole, int>
    {
        public AppIdentityContext(DbContextOptions<AppIdentityContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; } //? add associations: userId -> UserAccount

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>()
                .HasKey(rt => rt.Refresh);

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.UserAccount)
                .WithMany(ua => ua.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            //builder.Entity<UserAccount>()
            //    .Ignore(ua => ua.Profile);

            //ToDo: 'Warning' can be only 0-100 (maybe worth a try regExp)
            //ToDo: 'RegisteredAt' autoset created date
            //ToDo: 'LastOnlineAt' default value equal 'RegisteredAt'

            //? Need?
            //builder.Entity<UserAccount>()           .ToTable("Users");
            //builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            //builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            //builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            //builder.Entity<IdentityUserRole<int>>() .ToTable("UserRoles");
            //builder.Entity<UserRole>()              .ToTable("Roles");
            //builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        }
    }
}