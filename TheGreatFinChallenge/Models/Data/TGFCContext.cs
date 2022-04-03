using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheGreatFinChallenge.Models;


namespace TheGreatFinChallenge.Models.Data
{
    public class TGFCContext : DbContext
    {

        // Initialize the Sjette constructor.
        public TGFCContext(DbContextOptions<TGFCContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne<Department>(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne<User>(a => a.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne<ActivityType>(a => a.ActivityType)
                .WithMany(a => a.Activities)
                .HasForeignKey(a => a.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityType>()
                .HasOne<Discipline>(a => a.Discipline)
                .WithMany(d => d.ActivityTypes)
                .HasForeignKey(a => a.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>()
                .HasOne<Directorate>(d => d.Directorate)
                .WithMany(d => d.Departments)
                .HasForeignKey(d => d.DirectorateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Image>()
                .HasOne<User>(i => i.User)
                .WithMany(u => u.Images)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            //modelBuilder.Entity<User>().Navigation(u => u.Department).AutoInclude();
            //modelBuilder.Entity<User>().Navigation(u => u.Activities).AutoInclude();
            //modelBuilder.Entity<User>().Navigation(u => u.Images).AutoInclude();

            //modelBuilder.Entity<Activity>().Navigation(a => a.User).AutoInclude();
            //modelBuilder.Entity<Activity>().Navigation(a => a.ActivityType).AutoInclude();

            //modelBuilder.Entity<ActivityType>().Navigation(a => a.Discipline).AutoInclude();
            //modelBuilder.Entity<ActivityType>().Navigation(a => a.Activities).AutoInclude();

            //modelBuilder.Entity<Image>().Navigation(i => i.User).AutoInclude();
            //modelBuilder.Entity<Image>().Navigation(i => i.Department).AutoInclude();

            //modelBuilder.Entity<Department>().Navigation(d => d.Directorate).AutoInclude();
            //modelBuilder.Entity<Department>().Navigation(d => d.Users).AutoInclude();
            //modelBuilder.Entity<Department>().Navigation(d => d.Images).AutoInclude();

            //modelBuilder.Entity<Directorate>().Navigation(d => d.Departments).AutoInclude();

            //modelBuilder.Entity<Discipline>().Navigation(d => d.ActivityTypes).AutoInclude();
        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Directorate> Directorate { get; set; }
        public DbSet<Discipline> Discipline { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<User> User { get; set; }

    }
}
