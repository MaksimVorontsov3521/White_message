using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webapi.Models;

namespace webapi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Message> Message { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

           
            modelBuilder.Entity<Message>()
           .HasOne(m => m.Sender)
           .WithMany(u => u.SentMessages)
           .HasForeignKey(m => m.SenderId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
