﻿// <auto-generated />
using Insurance.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Insurance.Api.Migrations
{
    [DbContext(typeof(InsuranceDbContext))]
    partial class InsuranceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("Insurance.Api.Data.Models.ProductTypeSurchargeRule", b =>
                {
                    b.Property<int>("ProductTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("FlatCartSurcharge")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("FlatItemSurcharge")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PercentageItemSurcharge")
                        .HasColumnType("TEXT");

                    b.HasKey("ProductTypeId");

                    b.ToTable("ProductTypeSurchargeRules");
                });
#pragma warning restore 612, 618
        }
    }
}
