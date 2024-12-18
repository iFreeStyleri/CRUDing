using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRUDing.DAL.Configurations
{
    public sealed class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Product);
            builder.HasOne(p => p.Order).WithMany(p => p.Products);
            builder.OwnsOne(p => p.CurrentCost);
        }
    }
}
