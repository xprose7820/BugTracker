using BugTracker.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Project
{
	public class ProjectCreate
	{
		
		public string Title { get; set; }
		
		public string Description { get; set; }
		
		public int ProjectManagerId { get; set; }
		
		// will be set in service
		//public int AdminId { get; set; }
		
	}
}
