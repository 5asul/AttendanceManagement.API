﻿// <auto-generated />
using System;
using AttendanceManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AttendanceManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241210031452_add_start_and_end_date_in_absentClass")]
    partial class add_start_and_end_date_in_absentClass
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AttendanceManagement.API.Models.AttendanceRecord", b =>
                {
                    b.Property<int>("AttendanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttendanceId"));

                    b.Property<int>("BarcodeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CheckIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckOut")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("AttendanceId");

                    b.HasIndex("BarcodeId");

                    b.HasIndex("WorkerId");

                    b.ToTable("AttendanceRecord");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Absence", b =>
                {
                    b.Property<int>("AbsenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AbsenceId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("AbsenceId");

                    b.HasIndex("WorkerId");

                    b.ToTable("Absences");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Attendance", b =>
                {
                    b.Property<int>("AttendanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttendanceId"));

                    b.Property<DateTime>("CheckIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CheckOut")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("AttendanceId");

                    b.HasIndex("WorkerId");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Barcode", b =>
                {
                    b.Property<int>("BarcodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BarcodeId"));

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("BarcodeValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BarcodeId");

                    b.HasIndex("AdminId");

                    b.ToTable("Barcodes");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("NotificationId");

                    b.HasIndex("WorkerId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AttendanceManagement.API.Models.AttendanceRecord", b =>
                {
                    b.HasOne("MyAttendanceApp.Models.Barcode", "Barcode")
                        .WithMany("Attendances")
                        .HasForeignKey("BarcodeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MyAttendanceApp.Models.User", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Barcode");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Absence", b =>
                {
                    b.HasOne("MyAttendanceApp.Models.User", "Worker")
                        .WithMany("Absences")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Attendance", b =>
                {
                    b.HasOne("MyAttendanceApp.Models.User", "Worker")
                        .WithMany("Attendances")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Barcode", b =>
                {
                    b.HasOne("MyAttendanceApp.Models.User", "Admin")
                        .WithMany("Barcodes")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Notification", b =>
                {
                    b.HasOne("MyAttendanceApp.Models.User", "Worker")
                        .WithMany("Notifications")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.Barcode", b =>
                {
                    b.Navigation("Attendances");
                });

            modelBuilder.Entity("MyAttendanceApp.Models.User", b =>
                {
                    b.Navigation("Absences");

                    b.Navigation("Attendances");

                    b.Navigation("Barcodes");

                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
