using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CRUDing.DAL.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            var converter = new EnumToStringConverter<Role>();
            builder.HasKey(u => u.Id);
            builder.HasOne(u => u.Cart).WithOne(c => c.User);
            builder.HasMany(u => u.Orders).WithOne(o => o.User);
            builder.Property(u => u.Salt).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(255);
            builder.Property(u => u.Password).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Name).HasMaxLength(24).IsRequired();
            builder.Property(u => u.LastName).HasMaxLength(30).IsRequired();
            builder.Property(u => u.Patronymic).HasMaxLength(30).IsRequired(false);
            builder.Property(u => u.Role).HasConversion(converter);
            builder.HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);
        }
    }
}
