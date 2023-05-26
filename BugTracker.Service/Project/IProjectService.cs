using BugTracker.Model.Project;
using BugTracker.Model.Ticket;
using BugTracker.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BugTracker.Service.Project
{
	public interface IProjectService
	{
		public Task<IPagedList<ProjectListDetail>> GetAllProjectsAsync(int? projectPage);
		public Task<List<ProjectListDetail>> GetAllProjectsAsync();
		public Task<bool> CreateProjectAsync(ProjectCreate model);
		public Task<bool> EditProjectAsync(ProjectEdit model);
		public Task<ProjectEdit> GetProjectForEditByIdAsync(int projectId);
		public Task<List<ProjectListDetail_NumOfUsers_NumOfTickets>> GetAllProjects_NumOfUsers_NumOfTicketsAsync();
		public Task<List<ProjectListDetail>> GetAllProjectsByUserIdAsync(int userId);
		public Task<bool> AddUserToProjectAsync(ProjectUserEdit model);
		
		public Task<List<UserListDetail>> GetListOfUsersByProjectIdAsync(int projectId);
		public Task<List<TicketListDetail>> GetListOfTicketsByProjectIdAsync(int projectId);
		public Task<ProjectDetail_Personnel_Tickets> GetProjectDetailByIdAsync(int id, int? personnelPage, int? ticketPage);
		public Task<ProjectDetail_Personnel_Tickets> GetProjectByManagerIdAsync(int? personnelPage, int? ticketPage);
		Task<int> NumOfProjectsAsync();
	}
}
