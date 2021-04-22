namespace FastFood.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class FastFoodContext : DbContext
    {
        public FastFoodContext()
        {

        }

        public FastFoodContext(DbContextOptions<FastFoodContext> options)
            : base(options)
        {
        }

        //DbSets
        public DbSet<Category> Categories { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Position> Positions { get; set; }

        // OnConfiguring(missing -> connection string is in appsetings.json) it is there due to security issue, (when the project is uppload to GiHub connection string should not be visible by everyone). On appsetings.json is put on Git Ignore <- and this make the file locale/accessible but not loaded into GitHub

        // and OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ItemId });

            builder.Entity<Position>()
                .HasAlternateKey(p => p.Name);

            builder.Entity<Item>()
                .HasAlternateKey(i => i.Name);
        }
    }
}
