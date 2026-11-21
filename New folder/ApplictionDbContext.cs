using finalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace finalProject
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // CRUCIAL: Must call base.OnModelCreating for Identity tables to configure correctly
            base.OnModelCreating(builder);

            // Fix for Reservation decimal warning
            builder.Entity<Reservation_Management>()
                .Property(r => r.totalPrice)
                .HasPrecision(18, 2);

            // Fix for Room Type decimal warning
            builder.Entity<Room_Type_Management>()
                .Property(rt => rt.Price)
                .HasPrecision(18, 2);

            builder.Entity<Reservation_Management>()
                .HasOne(r=>r.Hotel) // or your navigation property name if you have one
                .WithMany()
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Reservation_Management>()
        .HasOne(r => r.Room) // Matches the 'public Room_Management Room' property in your model
        .WithMany()
        .HasForeignKey(r => r.RoomId)
        .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<Hotel_Management> Hotels { get; set; }
       public DbSet<Room_Type_Management> RoomTypes { get; set; }
       public DbSet<Room_Management> Rooms { get; set; }
      public  DbSet<Customer> Customers { get; set; }
       public DbSet<Reservation_Management> Reservations { get; set; }
    }
}
