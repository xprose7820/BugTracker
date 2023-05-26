using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.Entities
{
    public class TicketCommentEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }
        public TicketEntity Ticket { get; set; }
        [Required]
        [ForeignKey(nameof(Commenter))]
        public int CommenterId { get; set; }
        public ApplicationUser Commenter { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
     
        public DateTime CreatedDate { get; set; }
    }
}
