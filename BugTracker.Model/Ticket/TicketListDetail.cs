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
	public class TicketListDetail
	{
	
		public int Id { get; set; }
		
		public string Title { get; set; }
		
		public string Description { get; set; }
	
		public TicketPriority Priority { get; set; }
		
		public TicketStatus Status { get; set; }
		
		public TicketType Type { get; set; }
		
		
		public string DeveloperName { get; set; }
		
		public string SubmitterName { get; set; }
		
		
		public string ProjectName { get; set; }
		
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		

	}
}
