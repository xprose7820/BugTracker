using BugTracker.Data.Entities;
using BugTracker.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using BugTracker.Model.Project;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using BugTracker.Model.User;
using BugTracker.Model.Ticket;
using X.PagedList;

namespace BugTracker.Service.Project
{
	public class ProjectService : IProjectService
	{
		private readonly int _userId;

		private ApplicationDbContext _context;
		// need to check program.cs?
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public ProjectService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
			var value = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var validId = int.TryParse(value, out _userId);
			if (!validId)
				throw new Exception("Attempted to build without user Id claim.");


			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public async Task<List<ProjectListDetail>> GetAllProjectsAsync()
		{
			
			List<ProjectListDetail> details = await _context.Projects.Where(entity => entity.AdminId == _userId).Select(g => new ProjectListDetail
			{
				Id = g.Id,
				Title = g.Title,
				Description = g.Description,
				AssignedManager = g.ProjectManagerId == 0 ? "N/A" : g.ProjectManager.UserName

			}).ToListAsync();
			

			return details;
		}
		public async Task<IPagedList<ProjectListDetail>> GetAllProjectsAsync(int? projectPage)
		{
			int projectPageNumber = (projectPage ?? 1);
			List<ProjectListDetail> details = await _context.Projects.Where(entity => entity.AdminId == _userId).Select(g => new ProjectListDetail
			{
				Id = g.Id,
				Title = g.Title,
				Description = g.Description,
				AssignedManager = g.ProjectManagerId == 0 ? "N/A" : g.ProjectManager.UserName

			}).ToListAsync();
			IPagedList<ProjectListDetail> pagedProjectDetails = details.ToPagedList(projectPageNumber, 5);

			return pagedProjectDetails;
		}
		public async Task<bool> CreateProjectAsync(ProjectCreate model)
		{
			ProjectEntity newProject = new ProjectEntity
			{
				Title = model.Title,
				Description = model.Description,
				AdminId = _userId,
				ProjectManagerId = model.ProjectManagerId,
			};
			_context.Projects.Add(newProject);

			// Save changes to get the Id of the newly created project
			await _context.SaveChangesAsync();

			// Create a new UserProjectEntity and add it to the context
			UserProjectEntity newUserProject = new UserProjectEntity
			{
				UserId = _userId,
				ProjectId = newProject.Id // Here, newProject.Id should be the Id of the newly created project
			};


			_context.UserProjects.Add(newUserProject);
			await _context.SaveChangesAsync();
			UserProjectEntity newUserProject2 = new UserProjectEntity
			{
				UserId = model.ProjectManagerId,
				ProjectId = newProject.Id // Here, newProject.Id should be the Id of the newly created project
			};


			_context.UserProjects.Add(newUserProject2);

			int numChanges = await _context.SaveChangesAsync();
			return numChanges == 1;
			//return true;
		}
		public async Task<List<ProjectListDetail_NumOfUsers_NumOfTickets>> GetAllProjects_NumOfUsers_NumOfTicketsAsync()
		{
			// !!!!!! could also do it this way... after fix 
			//List<ProjectListDetail_NumOfUsers_NumOfTickets> details = await _context.Projects.Where(entity => entity.AdminId == _userId)
			//	.Select(async g => new ProjectListDetail_NumOfUsers_NumOfTickets
			//	{
			//		Id = g.Id,
			//		Title = g.Title,
			//		Description = g.Description,

			//		//UserName = await _userManager.FindByIdAsync(g.ProjectManagerId.ToString()).Username,
			//      UserName = g.
			//		NumberOfTickets = g.Tickets.Count(),
			//		NumberOfUsers = g.UserProjects.Count()
			//	}).ToListAsync();
			var projects = await _context.Projects
			.Include(g => g.Tickets)
			.Include(g => g.UserProjects)
			.Where(entity => entity.AdminId == _userId)
			.ToListAsync();

			List<ProjectListDetail_NumOfUsers_NumOfTickets> details = new List<ProjectListDetail_NumOfUsers_NumOfTickets>();

			foreach (ProjectEntity g in projects)
			{
				var user = await _userManager.FindByIdAsync(g.ProjectManagerId.ToString());

				details.Add(new ProjectListDetail_NumOfUsers_NumOfTickets
				{

					Id = g.Id,
					Title = g.Title,
					Description = g.Description,
					//UserName = g.ProjectManager.UserName,
					UserName = g.ProjectManagerId == 0 ? "N/A" : user.UserName,
					// make sure to .include
					NumberOfTickets = g.Tickets.Count,
					NumberOfUsers = g.UserProjects.Count

				});
			}

			return details;
		}

		public async Task<List<ProjectListDetail>> GetAllProjectsByUserIdAsync(int userId)
		{
			List<ProjectListDetail> details = await _context.Users.Where(entity => entity.Id == userId).Include(g => g.UserProjects)
				.ThenInclude(up => up.Project).SelectMany(g => g.UserProjects).Select(u => new ProjectListDetail
				{
					Id = u.Project.Id,
					Title = u.Project.Title,
					Description = u.Project.Description,
				}).ToListAsync();
			return details;
		}
		public async Task<bool> AddUserToProjectAsync(ProjectUserEdit model)
		{
			ProjectEntity project = await _context.Projects.FindAsync(model.ProjectId);
			ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(g => g.Id == model.UserId);
			IList<string> userRoles = await _userManager.GetRolesAsync(user);
			//check if the current project already has a manager
			UserProjectEntity projectHasManager = await _context.UserProjects.FindAsync(project.ProjectManagerId, model.ProjectId);
			// check if the chosen manager has a project
			UserProjectEntity managerHasProject = await _context.UserProjects.FirstOrDefaultAsync(g => g.UserId == model.UserId);

			if (userRoles[0] == "Project_Manager")
			{
				if (projectHasManager != null && managerHasProject != null)
				{
					int previousManagerId = projectHasManager.UserId;
					int thisManager_ProjectId = managerHasProject.ProjectId;

					_context.UserProjects.Remove(projectHasManager);
					_context.UserProjects.Remove(managerHasProject);
					await _context.SaveChangesAsync();

					ProjectEntity currentManagerProject = await _context.Projects.FindAsync(thisManager_ProjectId);

					currentManagerProject.ProjectManagerId = 0;
					project.ProjectManagerId = model.UserId;

					UserProjectEntity newProject = new UserProjectEntity
					{
						UserId = model.UserId,
						ProjectId = model.ProjectId
					};
					_context.UserProjects.Add(newProject);
					int numOfChanges = await _context.SaveChangesAsync();
					return numOfChanges == 3;
				}
				else if (projectHasManager != null && managerHasProject == null)
				{
					int previousManagerId = projectHasManager.UserId;
					_context.UserProjects.Remove(projectHasManager);
					await _context.SaveChangesAsync();

					project.ProjectManagerId = model.UserId;
					UserProjectEntity newProject = new UserProjectEntity
					{
						UserId = model.UserId,
						ProjectId = model.ProjectId
					};
					_context.UserProjects.Add(newProject);
					int numOfChanges = await _context.SaveChangesAsync();
					return numOfChanges == 2;

				}
				else if (projectHasManager == null && managerHasProject == null)
				{
					project.ProjectManagerId = model.UserId;
					UserProjectEntity newProject = new UserProjectEntity
					{
						UserId = model.UserId,
						ProjectId = model.ProjectId
					};
					_context.UserProjects.Add(newProject);
					int numOfChanges = await _context.SaveChangesAsync();
					return numOfChanges == 2;
				}
				else if (projectHasManager == null && managerHasProject != null)
				{
					int thisManager_ProjectId = managerHasProject.ProjectId;
					ProjectEntity currentManagerProject = await _context.Projects.FindAsync(thisManager_ProjectId);

					_context.UserProjects.Remove(managerHasProject);
					await _context.SaveChangesAsync();

					currentManagerProject.ProjectManagerId = 0;
					project.ProjectManagerId = model.UserId;

					UserProjectEntity newProject = new UserProjectEntity
					{
						UserId = model.UserId,
						ProjectId = model.ProjectId
					};
					_context.UserProjects.Add(newProject);
					int numOfChanges = await _context.SaveChangesAsync();
					return numOfChanges == 3;
				}



			}
			else
			{
				UserProjectEntity newProject = new UserProjectEntity
				{
					UserId = model.UserId,
					ProjectId = model.ProjectId
				};
				_context.UserProjects.Add(newProject);
				int numOfChanges = await _context.SaveChangesAsync();
				return numOfChanges == 1;
			}
			return false;

		}


		public async Task<ProjectDetail_Personnel_Tickets> GetProjectByManagerIdAsync(int? personnelPage, int? ticketPage)
		{
			int personnelPageNumber = (personnelPage ?? 1);
			int ticketPageNumber = (ticketPage ?? 1);
			ProjectEntity project = await _context.Projects
				.Include(g => g.UserProjects)
				.Include(g => g.Tickets)
				.ThenInclude(ticket => ticket.Developer)
				.Include(g => g.Tickets)
				.ThenInclude(ticket => ticket.Submitter)
				.Include(g=>g.ProjectManager).
				Where(entity => entity.ProjectManagerId == _userId).FirstOrDefaultAsync();

			List<UserListDetail> users = await GetListOfUsersByProjectIdAsync(project.Id);
			ProjectDetail_Personnel_Tickets detail = new ProjectDetail_Personnel_Tickets
			{
				Id = project.Id,
				Title = project.Title,
				Description = project.Description,
				ProjectManagerName = project.ProjectManager.UserName,
				Personnel = users.OrderBy(c => c.UserName).ToPagedList(personnelPageNumber, 5),
				Tickets = project.Tickets.OrderBy(c => c.CreatedDate).ToPagedList(ticketPageNumber, 5)
			};
			return detail;
		}
		public async Task<ProjectDetail_Personnel_Tickets> GetProjectDetailByIdAsync(int id, int? personnelPage, int? ticketPage)
		{
			int personnelPageNumber = (personnelPage ?? 1);
			int ticketPageNumber = (ticketPage ?? 1);
			ProjectEntity project = await _context.Projects
				.Include(g => g.UserProjects)
				.Include(g => g.Tickets)
				.ThenInclude(ticket => ticket.Developer)
				.Include(g => g.Tickets)
				.ThenInclude(ticket => ticket.Submitter)
				.Include(g => g.ProjectManager)
				.Where(entity => entity.Id == id).FirstOrDefaultAsync();
			List<UserListDetail> users = await GetListOfUsersByProjectIdAsync(id);
			ProjectDetail_Personnel_Tickets detail = new ProjectDetail_Personnel_Tickets
			{
				Id = id,
				Title = project.Title,
				Description = project.Description,
				ProjectManagerName = project.ProjectManager.UserName,
				Personnel = users.OrderBy(c => c.UserName).ToPagedList(personnelPageNumber, 5),
				Tickets = project.Tickets.OrderBy(c => c.CreatedDate).ToPagedList(ticketPageNumber, 5)
			};
			return detail;

		}
		public async Task<List<UserListDetail>> GetListOfUsersByProjectIdAsync(int projectId)
		{
			List<ApplicationUser> users = await _context.UserProjects.Where(p => p.ProjectId == projectId).Select(p => p.User).ToListAsync();
		    List<UserListDetail> details = new List<UserListDetail>();
			foreach(ApplicationUser user in users)
			{
				var userRoles = await _userManager.GetRolesAsync(user);
				var firstRole = userRoles.FirstOrDefault();
				details.Add(new UserListDetail
				{
					Id = user.Id,
					UserName = user.UserName,
					Role = firstRole,
					Email = user.Email

				});
			}
			return details;
		}
		public async Task<List<TicketListDetail>> GetListOfTicketsByProjectIdAsync(int projectId)
		{
		
			List<ProjectEntity> projects = await _context.Projects
				.Include(entity => entity.Tickets)
				.ThenInclude(ticket => ticket.Developer)
				.Include(entity => entity.Tickets)
				.ThenInclude(ticket => ticket.Submitter)
				.Where(g => g.Id == projectId)
				.ToListAsync();
			List<TicketListDetail> ticketDetails = new List<TicketListDetail>();

			foreach (ProjectEntity project in projects)
			{
				List<TicketEntity> tickets = project.Tickets.ToList();
				foreach (TicketEntity ticket in tickets)
				{

					ticketDetails.Add(new TicketListDetail
					{
						Id = ticket.Id,
						Title = ticket.Title,
						Description = ticket.Description,
						Priority = ticket.Priority,
						Status = ticket.Status,
						Type = ticket.Type,
						DeveloperName = ticket.Developer.UserName,
						SubmitterName = ticket.Submitter.UserName,
						ProjectName = project.Title,
						CreatedDate = ticket.CreatedDate,
						UpdatedDate = ticket.UpdatedDate,

					});

				}
			}
			return ticketDetails;
		}


		public async Task<bool> EditProjectAsync(ProjectEdit model)
		{
			ProjectEntity project = await _context.Projects.Where(entity => entity.Id == model.Id).FirstOrDefaultAsync();
			project.Title = model.Title;
			project.Description = model.Description;
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<ProjectEdit> GetProjectForEditByIdAsync(int projectId)
		{
			ProjectEntity project = await _context.Projects.Where(entity => entity.Id == projectId).FirstOrDefaultAsync();
			ProjectEdit detail = new ProjectEdit
			{
				Id = projectId,
				Title = project.Title,
				Description = project.Description,
			};
			return detail;


		}
	

		public async Task<int> NumOfProjectsAsync()
		{
			var user = await _userManager.FindByIdAsync(_userId.ToString());
			var roles = await _userManager.GetRolesAsync(user);
			int num = 0;


			if (roles[0] == "Admin")
			{
				num = _context.Projects.Where(g => g.AdminId == _userId).Count();
			}
			if (roles[0] == "Project_Manager")
			{
				num = _context.Projects.Where(g => g.ProjectManagerId == _userId).Count();
			}
			return num;
		}
	}
}
