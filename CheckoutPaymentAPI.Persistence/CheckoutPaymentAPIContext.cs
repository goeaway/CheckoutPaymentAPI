using CheckoutPaymentAPI.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Persistence
{
    public class CheckoutPaymentAPIContext : DbContext
    {
        public CheckoutPaymentAPIContext(DbContextOptions<CheckoutPaymentAPIContext> options) : base(options)
        {

        }

        public DbSet<ProcessedPayment> ProcessedPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedPayment>().ToTable("ProcessedPayment");
        }
    }
}
