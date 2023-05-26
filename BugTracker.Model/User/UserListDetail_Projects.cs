using BugTracker.Model.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.User
{
	public class UserListDetail_Projects
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		
		public string Role { get; set; }

		public List<ProjectListDetail> Projects { get; set; }
	}
}
