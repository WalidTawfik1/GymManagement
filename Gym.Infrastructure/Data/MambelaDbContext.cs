using Gym.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Data
{
    public class MambelaDbContext: DbContext
    {
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }

        public MambelaDbContext(DbContextOptions<MambelaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.Memberships)
                .WithOne(m => m.Trainee)
                .HasForeignKey(m => m.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trainee>()
                .HasMany(t => t.Visits)
                .WithOne(v => v.Trainee)
                .HasForeignKey(v => v.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Staff>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Staff)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Staff>().ToTable("Staff"); // avoid "Staffs"

        }
    }
}
