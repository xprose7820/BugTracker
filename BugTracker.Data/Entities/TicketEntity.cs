using BugTracker.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.Entities
{
    public class TicketEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public TicketPriority Priority { get; set; }
        [Required]
        public TicketStatus Status { get; set; }
        [Required]
        public TicketType Type { get; set; }
        [Required]
        [ForeignKey(nameof(Developer))]
        public int DeveloperId { get; set; }
        public ApplicationUser Developer { get; set; }
        [Required]
        [ForeignKey(nameof(Submitter))]
        public int SubmitterId { get; set; }
        public ApplicationUser Submitter { get; set; }

        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public ProjectEntity Project { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<TicketHistoryEntity> TicketHistories { get; set; }
        public List<TicketCommentEntity> TicketComments { get; set; }

        


    }
}
