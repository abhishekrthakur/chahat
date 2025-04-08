using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using TaskManagmentSystem.Models;


namespace TaskManagmentSystem.Data
{
    public class TaskManagmentDBContext : DbContext
    {
        public TaskManagmentDBContext(DbContextOptions<TaskManagmentDBContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<TeamMembers> TeamMembers { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachments>()
                .HasKey(a => a.AttachmentId);

            modelBuilder.Entity<User>()
                .HasKey(a => a.UserId);

            modelBuilder.Entity<Teams>()
                .HasKey(a => a.TeamId);

            modelBuilder.Entity<Notes>()
                .HasKey(a => a.NoteId); 
            
            modelBuilder.Entity<Tasks>()
                .HasKey(a => a.TaskId);
            
            modelBuilder.Entity<TeamMembers>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Attendance>()
                .HasKey(a => a.Id);
        }
    }
}
