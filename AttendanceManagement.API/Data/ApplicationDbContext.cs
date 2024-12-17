using AttendanceManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Barcode> Barcodes { get; set; } = null!;
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Absence> Absences { get; set; } = null!;
        public DbSet<WorkTime> WorkTimes { get;set;}=null!;
        public DbSet<UserWorkTime> UserWorkTimes { get;set;}=null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Relationships:

            // One admin -> Many barcodes
            modelBuilder.Entity<User>()
                .HasMany(u => u.Barcodes)
                .WithOne(b => b.Admin)
                .HasForeignKey(b => b.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // One worker -> Many attendance records
            modelBuilder.Entity<User>()
                .HasMany(u => u.Attendances)
                .WithOne(a => a.Worker)
                .HasForeignKey(a => a.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One worker -> Many notifications
            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.Worker)
                .HasForeignKey(n => n.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One worker -> Many absences
            modelBuilder.Entity<User>()
                .HasMany(u => u.Absences)
                .WithOne(ab => ab.Worker)
                .HasForeignKey(ab => ab.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One barcode -> Many attendance records
            modelBuilder.Entity<Barcode>()
                .HasMany(b => b.Attendances)
                .WithOne(a => a.Barcode)
                .HasForeignKey(a => a.BarcodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship configuration
            modelBuilder.Entity<UserWorkTime>()
                .HasKey(uwt => new { uwt.UserId, uwt.WorkTimeId });

            modelBuilder.Entity<UserWorkTime>()
                .HasOne(uwt => uwt.User)
                .WithMany(u => u.UserWorkTimes)
                .HasForeignKey(uwt => uwt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkTime>()
                .HasOne(uwt => uwt.WorkTime)
                .WithMany(wt => wt.UserWorkTimes)
                .HasForeignKey(uwt => uwt.WorkTimeId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}


   
