using BugTracker.Data.Entities;
using BugTracker.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Ticket
{
	public class TicketCreate
	{

		public string Title { get; set; }
		
		public string Description { get; set; }
		
		public TicketPriority Priority { get; set; }
		
		
		
		public TicketType Type { get; set; }
		
		
		public int DeveloperId { get; set; }
		
		
		public int SubmitterId { get; set; }
		
		public int ProjectId { get; set; }
		
		public DateTime CreatedDate { get; set; }
		
	}
}
