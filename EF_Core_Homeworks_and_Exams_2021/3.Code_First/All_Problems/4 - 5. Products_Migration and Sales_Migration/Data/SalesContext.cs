using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }

        // this constructor below is needed by Judge - must
        public SalesContext(DbContextOptions options) 
            : base(options)
        {
        }


        // DbSets (even without DbSets EFCore will create DB, but the tables will be named in sigulare)
        // A DbSet (<Customer> Customers, for example) represents a table from the DataBase (all records from the table), and the class Customer represents a row from the table (an entity)
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }


        // OnConficuring & OnModelCreating
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Settings.ConnectionString);
            }

            //base.OnConfiguring(optionsBuilder); // <-- delete this ?!?!
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder); // <-- delete this

            modelBuilder.Entity<Product>(product =>
            {
                //Primary Key
                product.HasKey(p => p.ProductId);

                //Name (up to 50 characters, unicode)
                product
                    .Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);

                //Quantity (real number)
                product
                    .Property(p => p.Quantity)
                    .IsRequired(true);

                //Price
                product
                    .Property(p => p.Price)
                    .IsRequired(true);

                // Description - up to 250 symbols, default value "No description".
                product
                    .Property(p => p.Description)
                    .HasMaxLength(250)
                    .IsRequired(false)
                    .IsUnicode(true)
                    .HasDefaultValue("No description");

                //Sales - ICollection (describe the relation, here the start is from Many to One)
                // and no need to describe the relation on both sides

                //product
                //    .HasMany(p => p.Sales) // in the class Product have ICollection<Sales> - many
                //    .WithOne(s => s.Product) // one
                //    .HasForeignKey(s => s.ProductId);
                //  //.OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>(customer => 
            {
                // Primary Key
                customer.HasKey(c => c.CustomerId);

                // Name
                customer
                    .Property(c => c.Name)
                    .HasMaxLength(100)
                    .IsRequired(true)
                    .IsUnicode(true);

                //	Email (up to 80 characters, not unicode)
                customer
                    .Property(c => c.Email)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(false);

                // CreditCardNumber (string)
                customer
                    .Property(c => c.CreditCardNumber)
                    .HasMaxLength(30) // numbers on the credit card  //if not MaxLength -> max
                    .IsRequired(true)
                    .IsUnicode(false);

                // Sales - ICollection - (describe the relation, start from One to Many - do not describe here)
            });

            modelBuilder.Entity<Sale>(sales =>
            {
                //Primary Key
                sales.HasKey(s => s.SaleId);

                // Date
                sales
                    .Property(s => s.Date)
                    .IsRequired(true)
                    .HasColumnType("DATETIME2")
                    .HasDefaultValueSql("GETDATE()");

                // Product - (describe the relation, start from One to Many)
                // and no ICollection<Product> in the class Sale
                sales
                    .HasOne(s => s.Product)
                    .WithMany(p => p.Sales) // in the class Product have ICollection<Sales>
                    .HasForeignKey(s => s.ProductId); // here is the same as .HasOne(s => s.Product)
                //.OnDelete(DeleteBehavior.Restrict);

                // Customer - (describe the relation, start from One to Many)
                // and no ICollection<Customer> in the class Sale
                sales
                    .HasOne(s => s.Customer)
                    .WithMany(c => c.Sales) // in the class Product have ICollection<Customer>
                    .HasForeignKey(s => s.CustomerId); // here in the class Sale can find the CustomerId
                //.OnDelete(DeleteBehavior.Restrict);

                // Store - (describe the relation, start from One to Many)
                // and no ICollection<Store> in the class Sale
                sales
                    .HasOne(s => s.Store)
                    .WithMany(st => st.Sales) // in the class Product have ICollection<Store>
                    .HasForeignKey(s => s.StoreId); // here in the class Sale can find the StoreId 
                    //.OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Store>(store =>
            {
                // Primary Key
                store.HasKey(s => s.StoreId);

                // Name (up to 80 characters, unicode)
                store
                    .Property(s => s.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);

                // Sales - ICollection (describe the relation, start from One to Many - do not describe here)
            });
        }
    }
}
