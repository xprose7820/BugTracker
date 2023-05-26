using BugTracker.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Ticket
{
	public class TicketEdit
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public TicketPriority Priority { get; set; }

		public TicketStatus Status { get; set; }

		public TicketType Type { get; set; }


		public string DeveloperName { get; set; }

		

		
		
	}
}
