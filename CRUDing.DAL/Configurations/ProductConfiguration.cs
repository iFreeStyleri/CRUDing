using CRUDing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRUDing.DAL.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Category).WithMany(p => p.Products);
            builder.OwnsOne(p => p.Cost);
            builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
            builder
                .HasMany(p => p.Carts)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
