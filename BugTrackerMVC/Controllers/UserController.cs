using BugTracker.Model.User;
using BugTracker.Service.User;
using BugTrackerMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Smo;

namespace BugTrackerMVC.Controllers
{
	// mostly for Admin? should get list of all users, be able to assign roles // view will have a userlistmodel and userassignrolemodel
	public class UserController : Controller
	{
		private IUserService _service;
		
		public UserController(IUserService service)
		{
			_service = service;
		}
		[HttpGet]
		public async Task<IActionResult> ManageUserRoles(int? userPage)
		{
			List<UserListDetail> users = await _service.GetListOfAllUsersAsync();
			List<RoleListDetail> roles = await _service.GetListOfAllRolesAsync();
			ManageUserRolesViewModel viewModel = new ManageUserRolesViewModel {
				ListOfAllUsers = await _service.GetListOfAllUsersAsync(userPage),
				UserRoleUpdateForm = new UserRoleUpdate(),

				Users = new SelectList(users.Select(u => new SelectListItem
				{
					Value = u.Id.ToString(),
					Text = u.UserName
				}), "Value", "Text"),
				Roles = new SelectList(roles.Select(r => new SelectListItem
				{
					Value = r.Id.ToString(),
					Text = r.Name
				}),"Value","Text")


			};
			return View("~/Views/Admin/ManageUserRoles.cshtml", viewModel);
		}

		[HttpPost]
		// 
		public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel unzipUserRoleUpdate)
		{
			UserRoleUpdate blah = unzipUserRoleUpdate.UserRoleUpdateForm;
			//if (!ModelState.IsValid)
			//{
			//	return View("~/Views/Admin/ManageUserRoles.cshtml", unzipUserRoleUpdate);
			//}
			bool isUpdated = await _service.AssignUserRoleAsync(blah);
			// if stateent not needed 
			
			//if (isUpdated)
			//{
			//	List<UserListDetail> users = await _service.GetListOfAllUsersAsync();
			//	List<RoleListDetail> roles = await _service.GetListOfAllRolesAsync();
			//	ManageUserRolesViewModel viewModel = new ManageUserRolesViewModel
			//	{
			//		ListOfAllUsers = users,
			//		UserRoleUpdateForm = new UserRoleUpdate(),

			//		Users = new SelectList(users.Select(u => new SelectListItem
			//		{
			//			Value = u.Id.ToString(),
			//			Text = u.UserName
			//		}), "Value", "Text"),
			//		Roles = new SelectList(roles.Select(r => new SelectListItem
			//		{
			//			Value = r.Id.ToString(),
			//			Text = r.Name
			//		}), "Value", "Text")


			//	};
			//	//return View("~/Views/Admin/ManageUserRoles.cshtml");
			//	return RedirectToAction("ManageUserRoles", "Admin");

			//}
			return RedirectToAction("ManageUserRoles", "User");
			//return View(blah);



		}



	}
}
