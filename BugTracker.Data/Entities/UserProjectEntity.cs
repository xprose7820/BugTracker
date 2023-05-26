using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.Entities
{
	public class UserProjectEntity
	{
		[ForeignKey(nameof(User))]
		public int UserId { get; set; }
		public ApplicationUser User { get; set; }
		[ForeignKey(nameof(Project))]
		public int ProjectId { get; set; }
		public ProjectEntity Project { get; set; }
	}
}
