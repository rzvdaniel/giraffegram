﻿// <auto-generated />
using System;
using GG.ComingSoon.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GG.ComingSoonMigrations.MsSql.Migrations
{
    [DbContext(typeof(EmailSubscriptionDbContext))]
    [Migration("20231228203357_ComingSoonFirst")]
    partial class ComingSoonFirst
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GG.ComingSoon.Core.EmailSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(319)
                        .HasColumnType("nvarchar(319)");

                    b.Property<DateTime>("SubscribedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("EmailSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}