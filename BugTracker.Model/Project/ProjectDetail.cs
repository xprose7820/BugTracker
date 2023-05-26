﻿using BugTracker.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Model.Project
{
	public class ProjectDetail
	{
		
		public int Id { get; set; }
	
		public string Title { get; set; }
		
		public string Description { get; set; }
		
		public string ProjectManagerName { get; set; }
		

		
	}
}
