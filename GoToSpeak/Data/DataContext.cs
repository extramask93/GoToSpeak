﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoToSpeak.Data
{
    public class DataContext : IdentityDbContext<User,Role,
    int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {          
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms {get; set;}
        public DbSet<Log> Logs {get; set;}

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
              builder.Entity<UserRole>(userRole => 
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(u => u.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(u => u.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

    

        }
    }
}
