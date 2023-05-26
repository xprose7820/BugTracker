using BugTracker.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.TicketComment
{
	public class TicketCommentCreate
	{
		
		public int TicketId { get; set; }
		
		
	
		public string Message { get; set; }
	

		
	}
}
