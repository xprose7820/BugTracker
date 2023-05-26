using BugTracker.Data.Entities;
using BugTracker.Model.Project;
using BugTracker.Model.User;
using BugTracker.Service.Project;
using BugTracker.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Management.Smo;
using X.PagedList;

namespace BugTrackerMVC.Controllers
{
	public class ProjectController : Controller
	{
		private IProjectService _service;
		private IUserService _userService;
		public ProjectController(IProjectService service, IUserService userService)
		{
			_service = service;
			_userService = userService;
		}
		[HttpGet]
		public async Task<IActionResult> MyProjects(int? projectPage)
		{
			ViewBag.Projects = await _service.GetAllProjectsAsync(projectPage);

			return View("~/Views/Admin/MyProjects.cshtml");
		}
		[HttpGet] 
		public async Task<IActionResult> CreateProject()
		{
			ProjectCreate notSure = new ProjectCreate();
			var users = await _userService.GetListOfAllUnasssignedProjectManagersAsync();
			ViewBag.ProjectManagers = new SelectList(users.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.UserName

			}), "Value", "Text");
			return View("~/Views/Admin/CreateProject.cshtml", notSure);
		}
		[HttpPost]
		public async Task<IActionResult> CreateProject(ProjectCreate model)
		{
			// didn't add model state cuz errors
			//when done, go back to the my project view 
			bool created = await _service.CreateProjectAsync(model);
			if (created)
			{
				// instead of using view again, redirect
				return RedirectToAction("MyProjects", "Project");
			}
			// !!!!!!!!!!!!!! pls do not forget about this, manage the bool 
			//return View(model);
			return RedirectToAction("MyProjects", "Project");
		}
		[HttpGet]
		public async Task<IActionResult> ManageProjectPersonnel(int? projectPage)
		{
			var projects = await _service.GetAllProjectsAsync();
			var users = await _userService.GetListOfAllUsersAsync(); 
			
			ViewBag.Users = await _userService.GetListOfAllUsers_ProjectsAsync(projectPage);
			ViewBag.Dropdown_Projects = new SelectList(projects.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.Title
			}), "Value", "Text");
			ViewBag.Dropdown_Users = new SelectList(users.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.UserName
			}), "Value", "Text");

			return View("~/Views/Admin/ManageProjectPersonnel.cshtml");

		}
		[HttpPost]
		public async Task<IActionResult> ManageProjectPersonnel(ProjectUserEdit model)
		{
			// make sure to handle manager differently
			bool userChanged = await _service.AddUserToProjectAsync(model);

			return RedirectToAction("ManageProjectPersonnel", "Project");


		}
		
		[HttpGet] 
		public async Task<IActionResult> EditProject(int id)
		{
			ProjectEdit detail = await _service.GetProjectForEditByIdAsync(id);

			return View("~/Views/Project/EditProject.cshtml", detail);
		}
		[HttpPost]
		public async Task<IActionResult> EditProject(ProjectEdit model)
		{
			bool isEdited = await _service.EditProjectAsync(model);
			if (isEdited)
			{
				if (User.IsInRole("Admin") || User.IsInRole("Demo_Admin")
				{
					return RedirectToAction("MyProjects", "Project");

				}
				
				
			}
			return View(ModelState);
		}

		[HttpGet]
		public async Task<IActionResult> DetailProject(int id, int? personnelPage, int? ticketPage)
		{
			ProjectDetail_Personnel_Tickets detail = await _service.GetProjectDetailByIdAsync(id, personnelPage, ticketPage);
			return View("~/Views/Project/DetailProject.cshtml", detail);
		}
		[HttpGet]
		public async Task<IActionResult> ManageMyProject_ProjectManager(int? personnelPage, int? ticketPage)
		{
			ProjectDetail_Personnel_Tickets detail = await _service.GetProjectByManagerIdAsync(personnelPage, ticketPage);

			
			return View("~/Views/Project/DetailProject.cshtml", detail);
		}

	}
}
