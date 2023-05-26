using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.Entities
{
    public class ProjectEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [ForeignKey(nameof(ProjectManager))]
        public int ProjectManagerId { get; set; }
        public ApplicationUser ProjectManager { get; set; }
		[Required]
		[ForeignKey(nameof(Admin))]
		public int AdminId { get; set; }
		public ApplicationUser Admin { get; set; }
		
        // really for the Developer and Submitter, manager only assigned to one project
        public virtual List<UserProjectEntity> UserProjects { get; set; }
        public virtual List<TicketEntity>  Tickets { get; set; }

    }
}
