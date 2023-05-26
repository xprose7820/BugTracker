using BugTracker.Data;
using BugTracker.Data.Entities;
using BugTracker.Data.Enum;
using BugTracker.Model.Project;
using BugTracker.Model.Ticket;
using BugTracker.Model.TicketComment;
using BugTracker.Model.User;
using BugTracker.Service.Project;
using BugTracker.Service.Ticket;
using BugTracker.Service.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Web.Helpers;

namespace BugTrackerMVC.Controllers
{
	public class TicketController : Controller
	{
		private ITicketService _ticketService;
		private IUserService _userService;
		private IProjectService _projectService;
		public TicketController(ITicketService ticketService, IUserService userService, IProjectService projectService)
		{

			_ticketService = ticketService;
			_userService = userService;
			_projectService = projectService;
		}
		[HttpGet]
		public async Task<IActionResult> MyTickets(int? myTicketPage)
		{
			ViewBag.TicketDetails = await _ticketService.GetListOfAllTicketsAsync(myTicketPage);

			return View("~/Views/Ticket/MyTickets.cshtml");
		}

		[HttpGet]
		public async Task<IActionResult> CreateTicket()
		{
			List<UserListDetail_Basic> developers = await _userService.GetListOfAllDevelopersAsync();
			List<UserListDetail_Basic> submitters = await _userService.GetListOfAllSubmittersAsync();
			List<ProjectListDetail> projects = await _projectService.GetAllProjectsAsync();
			ViewBag.Dropdown_Priority = new SelectList(Enum.GetValues(typeof(TicketPriority)));
			ViewBag.Dropdown_Type = new SelectList(Enum.GetValues(typeof(TicketType)));
			ViewBag.Dropdown_Developers = new SelectList(developers.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.UserName
			}), "Value", "Text");
			ViewBag.Dropdown_Submitters = new SelectList(submitters.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.UserName
			}), "Value", "Text");
			ViewBag.Dropdown_Projects = new SelectList(projects.Select(u => new SelectListItem
			{
				Value = u.Id.ToString(),
				Text = u.Title
			}), "Value", "Text");
			return View("~/Views/Admin/CreateTicket.cshtml");

		}
		[HttpPost]
		public async Task<IActionResult> CreateTicket(TicketCreate model)
		{
			bool isCreated = await _ticketService.CreateTicketAsync(model);
			if (isCreated)
			{
				return RedirectToAction("MyTickets", "Ticket");
			}
			return View(model);
		}

		//[HttpGet]
		//public async Task<IActionResult> MyTickets_ProjectManager()
		//{
		//	ProjectDetail details = await _projectService.GetProjectByManagerIdAsync();
		//	ViewBag.TicketDetails = await _projectService.GetListOfTicketsByProjectIdAsync(details.Id);
		//	return View("~/Views/Ticket/MyTickets.cshtml");
		//}
		[HttpGet]
		public async Task<IActionResult> MyTickets_Developer()
		{
			ViewBag.TicketDetails = await _ticketService.GetListOfTicketsByDeveloperIdAsync();
			return View("~/Views/Ticket/MyTickets.cshtml");
		}
		[HttpGet]
		public async Task<IActionResult> ManageMyTickets_Submitter()
		{
			ViewBag.TicketDetails = await _ticketService.GetListOfTicketsBySubmitterIdAsync();
			return View("~/Views/Ticket/MyTickets.cshtml");
		}

		[HttpGet]
		public async Task<IActionResult> EditTicket(int id)
		{

			TicketEdit detail = await _ticketService.GetTicketForEditByIdAsync(id);

			List<UserListDetail_Basic> developers = await _userService.GetListOfAllDevelopersAsync();
			ViewBag.Dropdown_Developers = developers.Select(u => new SelectListItem
			{
				Value = u.UserName,
				Text = u.UserName,

				Selected = u.UserName == detail.DeveloperName

			}).ToList();
			ViewBag.Dropdown_Type = Enum.GetValues(typeof(TicketType)).Cast<TicketType>().Select(u => new SelectListItem
			{
				Value = u.ToString(),
				Text = u.ToString(),
				Selected = u == detail.Type
			}).ToList();

			ViewBag.Dropdown_Priority = Enum.GetValues(typeof(TicketPriority)).Cast<TicketPriority>().Select(u => new SelectListItem
			{
				Value = u.ToString(),
				Text = u.ToString(),
				Selected = u == detail.Priority
			}).ToList();
			ViewBag.Dropdown_Status = Enum.GetValues(typeof(TicketStatus)).Cast<TicketStatus>().Select(u => new SelectListItem
			{
				Value = u.ToString(),
				Text = u.ToString(),
				Selected = u == detail.Status
			}).ToList();

			return View("~/Views/Ticket/EditTicket.cshtml", detail);
		}
		[HttpPost]
		public async Task<IActionResult> EditTicket(int id, TicketEdit detail)
		{

			bool isEdited = await _ticketService.EditTicketAsync(id, detail);
			if (isEdited)
			{
				if (User.IsInRole("Admin") || User.IsInRole("Demo_Admin"))
				{
					//for admin view 
					return RedirectToAction("MyTickets", "Ticket");
				}
				if (User.IsInRole("Developer"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_Developer", "Ticket");
				}
				if (User.IsInRole("Submitter"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_Submitter", "Ticket");
				}
				if (User.IsInRole("Project_Manager"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_ProjectManager", "Ticket");
				}
			}
			return View(ModelState);
		}

		[HttpGet]
		public async Task<IActionResult> DetailTicket(int id, int? commentPage, int? historyPage)
		{
			TicketDetail detail = await _ticketService.GetTicketDetailByIdAsync(id, commentPage, historyPage);
			return View("~/Views/Ticket/DetailTicket.cshtml", detail);
		}
		[HttpGet]
		public async Task<IActionResult> CommentTicket(int id)
		{
			ViewBag.TicketId = id;
			return View("~/Views/Ticket/CommentTicket.cshtml");
		}
		[HttpPost] 
		public async Task<IActionResult> CommentTicket(TicketCommentCreate detail)
		{
			bool isCreated = await _ticketService.CommentTicketAsync(detail);
			if (isCreated)
			{
				if (User.IsInRole("Admin") || User.IsInRole("Demo_Admin"))
				{
					//for admin view 
					return RedirectToAction("MyTickets", "Ticket");
				}
				if (User.IsInRole("Developer"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_Developer", "Ticket");
				}
				if (User.IsInRole("Submitter"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_Submitter", "Ticket");
				}
				if (User.IsInRole("Project_Manager"))
				{
					//for admin view 
					return RedirectToAction("MyTickets_ProjectManager", "Ticket");
				}
			}
			return View(ModelState);
		}


		[HttpGet]
		public async Task<IActionResult> BarChart()
		{
			List<TicketStatusCount> ticketsByStatus = await _ticketService.GetTicketByStatus_BarChartAsync();
			List<TicketTypeCount> ticketsByType = await _ticketService.GetTicketByType_BarChartAsync();
			List<TicketPriorityCount> ticketsByPriority = await _ticketService.GetTicketByPriority_BarChartAsync();


			var statusLabels = ticketsByStatus.Select(g => g.Status.ToString()).ToArray();
            var statusData = ticketsByStatus.Select(g => g.Count).ToArray();

			var typeLabels = ticketsByType.Select(g => g.Type.ToString()).ToArray();
			var typeData = ticketsByType.Select(g => g.Count).ToArray();

			var priorityLabels = ticketsByPriority.Select(g => g.Priority.ToString()).ToArray();
			var priorityData = ticketsByPriority.Select(g => g.Count).ToArray();

			ViewBag.StatusLabels = Newtonsoft.Json.JsonConvert.SerializeObject(statusLabels);
            ViewBag.StatusData = Newtonsoft.Json.JsonConvert.SerializeObject(statusData);

			ViewBag.TypeLabels = Newtonsoft.Json.JsonConvert.SerializeObject(typeLabels);
			ViewBag.TypeData = Newtonsoft.Json.JsonConvert.SerializeObject(typeData);

			ViewBag.PriorityLabels = Newtonsoft.Json.JsonConvert.SerializeObject(priorityLabels);
			ViewBag.PriorityData = Newtonsoft.Json.JsonConvert.SerializeObject(priorityData);

			ViewBag.NumOfProjects = await _projectService.NumOfProjectsAsync() ;
			ViewBag.NumOfUsers = await _userService.NumOfUsersAsync() ;
			ViewBag.NumOfTickets = await _ticketService.NumOfTicketsAsync();

			return View("~/Views/Home/Index.cshtml");
        }



	}

}
