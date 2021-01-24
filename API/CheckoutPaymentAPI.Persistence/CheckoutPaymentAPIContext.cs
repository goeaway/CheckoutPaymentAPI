using CheckoutPaymentAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckoutPaymentAPI.Persistence
{
    /// <summary>
    /// Entity Framework Core context for the application
    /// </summary>
    public class CheckoutPaymentAPIContext : DbContext
    {
        public CheckoutPaymentAPIContext(DbContextOptions<CheckoutPaymentAPIContext> options) : base(options)
        {

        }

        /// <summary>
        /// Gets or sets processed payments
        /// </summary>
        public DbSet<ProcessedPayment> ProcessedPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedPayment>().ToTable("ProcessedPayment");
        }
    }
}
