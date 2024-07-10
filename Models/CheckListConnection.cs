using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Administration;
using System.Collections.Generic;

namespace CheckListJob.Models
{
    public class CheckListContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftTask> ShiftTasks { get; set; }
        public DbSet<ListLog> ListLogs { get; set; }
        public DbSet<ShiftTaskHst> ShiftTaskHsts { get; set; }


        public CheckListContext() 
        {
            //Database.EnsureDeleted();   // удаляем бд со старой схемой
            Database.EnsureCreated();   // создаем бд с новой схемой
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseMySQL(configuration.GetConnectionString("CheckDBConnection")!);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role admin = new() { Id = 1, Name = "admin" };
            modelBuilder.Entity<Role>().HasData(
                 admin,
                new Role { Id = 2, Name = "user" });
            modelBuilder.Entity<Shift>().HasData(
                new Shift { Id = 1, Name = "0 смена", StartTime = new TimeSpan(8, 30, 00), FinishTime = new TimeSpan(17, 30, 0), Description = "Какое-то описание" },
                new Shift { Id = 2, Name = "1 смена", StartTime = new TimeSpan(7, 30, 00), FinishTime = new TimeSpan(16, 30, 0), Description = "Какое-то описание" },
                new Shift { Id = 3, Name = "2 смена", StartTime = new TimeSpan(16, 00, 00), FinishTime = new TimeSpan(21, 00, 0), Description = "Какое-то описание" },
                new Shift {Id = 4, Name = "3 смена", StartTime = new TimeSpan(21, 00, 00), FinishTime = new TimeSpan(07, 30, 0, 0).Add(new TimeSpan(1, 0, 0, 0)), Description = "Какое-то описание" });
        }
    }
}
