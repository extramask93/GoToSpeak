using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoToSpeak.Models;
using Microsoft.EntityFrameworkCore;

namespace GoToSpeak.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {          
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms {get; set;}

        protected override void OnModelCreating(ModelBuilder builder) {
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
