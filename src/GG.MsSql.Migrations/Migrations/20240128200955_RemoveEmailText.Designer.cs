﻿// <auto-generated />
using System;
using GG.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GG.Migrations.MsSql.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240128200955_RemoveEmailText")]
    partial class RemoveEmailText
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GG.Core.Entities.ApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("GG.Core.Entities.ApiKeyUser", b =>
                {
                    b.Property<Guid>("ApiKeyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ApiKeyId", "UserId");

                    b.HasIndex(new[] { "ApiKeyId" }, "IX_ApiKeyUser_ApiKeyId");

                    b.ToTable("ApiKeyUsers");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Port")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("UseSsl")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("EmailAccounts");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailAccountUser", b =>
                {
                    b.Property<Guid>("EmailAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EmailAccountId", "UserId");

                    b.HasIndex(new[] { "EmailAccountId" }, "IX_EmailAccountUser_EmailAccountId");

                    b.ToTable("EmailAccountUsers");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Html")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplates");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailTemplateUser", b =>
                {
                    b.Property<Guid>("EmailTemplateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EmailTemplateId", "UserId");

                    b.HasIndex(new[] { "EmailTemplateId" }, "IX_EmailTemplateUser_EmailTemplateId");

                    b.ToTable("EmailTemplateUsers");
                });

            modelBuilder.Entity("GG.Core.Entities.ApiKeyUser", b =>
                {
                    b.HasOne("GG.Core.Entities.ApiKey", null)
                        .WithMany("ApiKeyUsers")
                        .HasForeignKey("ApiKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GG.Core.Entities.EmailAccountUser", b =>
                {
                    b.HasOne("GG.Core.Entities.EmailAccount", null)
                        .WithMany("EmailAccountUsers")
                        .HasForeignKey("EmailAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GG.Core.Entities.EmailTemplateUser", b =>
                {
                    b.HasOne("GG.Core.Entities.EmailTemplate", null)
                        .WithMany("EmailTemplateUsers")
                        .HasForeignKey("EmailTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GG.Core.Entities.ApiKey", b =>
                {
                    b.Navigation("ApiKeyUsers");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailAccount", b =>
                {
                    b.Navigation("EmailAccountUsers");
                });

            modelBuilder.Entity("GG.Core.Entities.EmailTemplate", b =>
                {
                    b.Navigation("EmailTemplateUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
