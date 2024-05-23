using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Configuration
{
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(80).IsRequired();

            builder.Property(p => p.LastName).HasMaxLength(80).IsRequired();

            builder.Property(p => p.Birthdate).IsRequired();

            builder.Property(p => p.Phone).HasMaxLength(9).IsRequired();

            builder.Property(p => p.Email).HasMaxLength(320).IsRequired();

            builder.Property(p => p.Address).HasMaxLength(120).IsRequired();

            builder.Property(p => p.Age);

            builder.Property(p => p.CreatedBy).HasMaxLength(30);

            builder.Property(p => p.LastUpdatedBy).HasMaxLength(30);
        }
    }
}
