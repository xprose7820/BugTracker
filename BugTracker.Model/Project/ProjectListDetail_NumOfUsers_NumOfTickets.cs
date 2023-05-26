using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Project
{
	public class ProjectListDetail_NumOfUsers_NumOfTickets
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string UserName { get; set; }
		public int NumberOfUsers { get; set; }
		public int NumberOfTickets { get; set; }
	}
}
