using BugTracker.Model.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using X.PagedList;

namespace BugTrackerMVC.Models
{
	// use viewbag?
	// view expects users and roles, so must pass into view
	public class ManageUserRolesViewModel
	{
		public IPagedList<UserListDetail> ListOfAllUsers { get; set; }	
		public UserRoleUpdate UserRoleUpdateForm { get; set; }
		public IEnumerable<SelectListItem> Users { get; set; }
		public IEnumerable<SelectListItem> Roles { get; set; }
	}
}


