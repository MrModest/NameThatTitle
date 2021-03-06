﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NameThatTitle.Data.DbContexts;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NameThatTitle.Data.Migrations.Forum
{
    [DbContext(typeof(ForumContext))]
    [Migration("20180716173258_InitialForum")]
    partial class InitialForum
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CommentId");

                    b.Property<string>("Link");

                    b.Property<int?>("PostId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("PostId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthorId");

                    b.Property<bool>("CorectAnswer");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("Hide");

                    b.Property<int?>("ParentId");

                    b.Property<int>("PostId");

                    b.Property<int>("Rating");

                    b.Property<string>("Text");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ParentId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Forum", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Forums");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthorId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("ForumId");

                    b.Property<bool>("Hide");

                    b.Property<int>("Rating");

                    b.Property<bool>("Solved");

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ForumId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Users.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<string>("Cover");

                    b.Property<int>("Rating");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Users.UserStatistic", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("MarkedAsSolve");

                    b.Property<int>("Solved");

                    b.Property<int>("UnmarkedAsSolve");

                    b.HasKey("Id");

                    b.ToTable("UserStatistics");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Attachment", b =>
                {
                    b.HasOne("NameThatTitle.Core.Models.Forum.Comment")
                        .WithMany("Attachments")
                        .HasForeignKey("CommentId");

                    b.HasOne("NameThatTitle.Core.Models.Forum.Post")
                        .WithMany("Attachments")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Comment", b =>
                {
                    b.HasOne("NameThatTitle.Core.Models.Users.UserProfile", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NameThatTitle.Core.Models.Forum.Comment", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId");

                    b.HasOne("NameThatTitle.Core.Models.Forum.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Forum.Post", b =>
                {
                    b.HasOne("NameThatTitle.Core.Models.Users.UserProfile", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NameThatTitle.Core.Models.Forum.Forum", "Forum")
                        .WithMany("Posts")
                        .HasForeignKey("ForumId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NameThatTitle.Core.Models.Users.UserStatistic", b =>
                {
                    b.HasOne("NameThatTitle.Core.Models.Users.UserProfile", "Profile")
                        .WithOne("Statistic")
                        .HasForeignKey("NameThatTitle.Core.Models.Users.UserStatistic", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
