using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.DAL.Configurations;
using CRUDing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDing.DAL
{
    public class CRUDContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductItem> ProductItems => Set<ProductItem>();
        public DbSet<Category> Categories => Set<Category>();

        public CRUDContext(DbContextOptions opt) : base(opt)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductItemConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
        }
    }
}
