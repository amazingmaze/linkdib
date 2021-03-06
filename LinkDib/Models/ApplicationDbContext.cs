﻿using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkDib.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Link> Links { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>().HasRequired(f => f.User).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Link>().HasRequired(f => f.User).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Following>().HasRequired(f => f.Follower).WithMany().WillCascadeOnDelete(false);

            // TODO: Kolla upp hur förhållandet verkligen fungerar. "Reverse relationships". 
            modelBuilder.Entity<UserNotification>().HasRequired(u => u.User).WithMany(u => u.UserNotifications).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}