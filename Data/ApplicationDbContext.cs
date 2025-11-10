using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System_Rezerwacji.Models;

namespace System_Rezerwacji.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Service)
            .WithMany(service => service.Reservations)
            .HasForeignKey(key => key.ServiceID);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Customer)
            .WithMany(customer => customer.Reservations)
            .HasForeignKey(key => key.CustomerID);

        modelBuilder.Entity<UserDetails>()
            .HasNoKey();
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<UserDetails> UserDetails { get; set; }
}
