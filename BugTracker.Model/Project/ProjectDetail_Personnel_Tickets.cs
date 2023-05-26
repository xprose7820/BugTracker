using BugTracker.Data.Entities;
using BugTracker.Model.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BugTracker.Model.Project
{
	public class ProjectDetail_Personnel_Tickets
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ProjectManagerName { get; set; }
		public IPagedList<UserListDetail> Personnel { get; set; }
		public IPagedList<TicketEntity> Tickets { get; set; }
	}
}
