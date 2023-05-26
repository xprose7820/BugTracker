using BugTracker.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BugTracker.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		public DbSet<ProjectEntity> Projects { get; set; }
		public DbSet<ApplicationUser> Users { get; set; }
		public DbSet<ApplicationRole> Roles { get; set; }
		public DbSet<TicketEntity> Tickets { get; set; }
		public DbSet<TicketHistoryEntity> TicketHistories { get; set; }
		public DbSet<TicketCommentEntity> TicketComments { get; set; }
		public DbSet<UserProjectEntity> UserProjects { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure the TicketEntity relationships
			modelBuilder.Entity<TicketEntity>()
				.HasOne(t => t.Developer)
				.WithMany()
				.HasForeignKey(t => t.DeveloperId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<TicketEntity>()
				.HasOne(t => t.Submitter)
				.WithMany()
				.HasForeignKey(t => t.SubmitterId)
				.OnDelete(DeleteBehavior.Restrict);
			//// MAJOR FIX not sure why 
			modelBuilder.Entity<TicketEntity>()
				.HasOne(t => t.Project)
				.WithMany(p=>p.Tickets)
				.HasForeignKey(t => t.ProjectId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ProjectEntity>()
				.HasOne(p => p.ProjectManager)
				.WithMany()
				.HasForeignKey(p => p.ProjectManagerId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ProjectEntity>()
				.HasOne(p => p.Admin)
				.WithMany()
				.HasForeignKey(p => p.AdminId)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<UserProjectEntity>()
		  .HasKey(up => new { up.UserId, up.ProjectId });
			// Add other configurations here
		}

	}
}