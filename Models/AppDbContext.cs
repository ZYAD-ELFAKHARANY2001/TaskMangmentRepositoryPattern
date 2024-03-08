using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Task_Mangment_Api.Models
{
    public class AppDbContext: IdentityDbContext<User>
    {
       public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
       {

       }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //modelBuilder.UseSerialColumns();
            builder.Entity<UserTask>().HasOne(ut => ut.User).WithMany(ut => ut.UserTasks).HasForeignKey(ut => ut.UserId);
            builder.Entity<UserTask>().HasOne(ut => ut.Task).WithMany(ut => ut.AssignedUsers).HasForeignKey(ut => ut.TaskId);
            builder.Entity<UserTask>().HasKey(ut => new { ut.UserId,ut.TaskId });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public DbSet<UserTask> UserTasks { get; set; }

    }
}
