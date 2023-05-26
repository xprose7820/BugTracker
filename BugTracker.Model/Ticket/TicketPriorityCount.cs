using BugTracker.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Ticket
{
	public class TicketPriorityCount
	{
		public TicketPriority Priority { get; set; }
		public int Count { get; set; }
	}
}
