using BugTracker.Data.Entities;
using BugTracker.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BugTracker.Model.Ticket;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using BugTracker.Model.TicketComment;
using X.PagedList;
using System.Drawing.Printing;

namespace BugTracker.Service.Ticket
{
	public class TicketService : ITicketService
	{

		private readonly int _userId;

		private ApplicationDbContext _context;
		// need to check program.cs?
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public TicketService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
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
		// !!!! grab current users id(admin) and from their grab their projects and get all tickets
		public async Task<List<TicketListDetail>> GetListOfAllTicketsAsync()
		{
			var user = await _userManager.FindByIdAsync(_userId.ToString());
			var roles = await _userManager.GetRolesAsync(user);
			if (roles[0] == "Admin" || roles[0] == "Project_Manager" || roles[0] == "Demo_Admin")
			{
				List<ProjectEntity> projects = new List<ProjectEntity>();

				if (roles[0] == "Admin" || roles[0] == "Demo_Admin")
				{
					projects = await _context.Projects
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Developer)
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Submitter)
						.Where(g => g.AdminId == _userId)
						.ToListAsync();
				}
				if (roles[0] == "Project_Manager")
				{
					projects = await _context.Projects
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Developer)
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Submitter)
						.Where(g => g.ProjectManagerId == _userId)
						.ToListAsync();
				}
				// why do we need theninclude if not virtual?

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
			if (roles[0] == "Developer")
			{

				List<TicketListDetail> tickets = await GetListOfTicketsByDeveloperIdAsync();
				
				return tickets;
			}
			return null;
		}

		// !!!! grab current users id(admin) and from their grab their projects and get all tickets
		public async Task<IPagedList<TicketListDetail>> GetListOfAllTicketsAsync(int? myTicketPage)
		{

			int myTicketPageNumber = (myTicketPage ?? 1);
			// why do we need theninclude if not virtual?
			var user = await _userManager.FindByIdAsync(_userId.ToString());
			var roles = await _userManager.GetRolesAsync(user);
			if (roles[0] == "Admin" || roles[0] == "Project_Manager" || roles[0] == "Demo_Admin")
			{
				List<ProjectEntity> projects = new List<ProjectEntity>();
				if (roles[0] == "Admin" || roles[0] == "Demo_Admin")
				{
					projects = await _context.Projects
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Developer)
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Submitter)
						.Where(g => g.AdminId == _userId)
						.ToListAsync();
				}
				if (roles[0] == "Project_Manager")
				{
					projects = await _context.Projects
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Developer)
						.Include(entity => entity.Tickets)
						.ThenInclude(ticket => ticket.Submitter)
						.Where(g => g.ProjectManagerId == _userId)
						.ToListAsync();
				}

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
				IPagedList<TicketListDetail> pagedTicketDetails = ticketDetails.ToPagedList(myTicketPageNumber, 5);

				return pagedTicketDetails;
			}
			if (roles[0] == "Developer")
			{
				
				List<TicketListDetail> tickets = await GetListOfTicketsByDeveloperIdAsync();
				IPagedList<TicketListDetail> pagedTicketDetails = tickets.ToPagedList(myTicketPageNumber, 5);
				return pagedTicketDetails;
			}
			return null;

		}

		public async Task<bool> CreateTicketAsync(TicketCreate model)
		{
			TicketEntity newTicket = new TicketEntity
			{
				Title = model.Title,
				Description = model.Description,
				Priority = model.Priority,
				Status = Data.Enum.TicketStatus.New,
				Type = model.Type,
				DeveloperId = model.DeveloperId,
				SubmitterId = model.SubmitterId,
				ProjectId = model.ProjectId,
				CreatedDate = DateTime.Now,
			};
			 _context.Tickets.Add(newTicket);
			int numberOfChanges = await _context.SaveChangesAsync();
			return numberOfChanges == 1;
		}

		public async Task<List<TicketListDetail>> GetListOfTicketsByDeveloperIdAsync()
		{
			List<TicketEntity> tickets = await _context.Tickets
				.Include(g=>g.Developer)
				.Include(g=>g.Submitter) 
				.Include(g=>g.Project)
				.Where(entity=> entity.DeveloperId == _userId)
				.ToListAsync();
			List<TicketListDetail> details = new List<TicketListDetail>();
			foreach(TicketEntity ticket in tickets)
			{
				details.Add(new TicketListDetail
				{
					Id = ticket.Id,
					Title = ticket.Title,
					Description = ticket.Description,
					Priority = ticket.Priority,
					Status = ticket.Status,
					Type = ticket.Type,
					DeveloperName = ticket.Developer.UserName,
					SubmitterName = ticket.Submitter.UserName,
					ProjectName = ticket.Project.Title,
					CreatedDate = ticket.CreatedDate,
					UpdatedDate = ticket.UpdatedDate,
				});
			}
			return details;
		}

		public async Task<List<TicketListDetail>> GetListOfTicketsBySubmitterIdAsync()
		{
			List<TicketEntity> tickets = await _context.Tickets
				.Include(g => g.Developer)
				.Include(g => g.Submitter)
				.Include(g => g.Project)
				.Where(entity => entity.SubmitterId == _userId)
				.ToListAsync();
			List<TicketListDetail> details = new List<TicketListDetail>();
			foreach (TicketEntity ticket in tickets)
			{
				details.Add(new TicketListDetail
				{
					Id = ticket.Id,
					Title = ticket.Title,
					Description = ticket.Description,
					Priority = ticket.Priority,
					Status = ticket.Status,
					Type = ticket.Type,
					DeveloperName = ticket.Developer.UserName,
					SubmitterName = ticket.Submitter.UserName,
					ProjectName = ticket.Project.Title,
					CreatedDate = ticket.CreatedDate,
					UpdatedDate = ticket.UpdatedDate,
				});
			}
			return details;
		}
		public async Task<TicketEdit> GetTicketForEditByIdAsync(int id)
		{
			TicketEntity ticket = await _context.Tickets.Include(g=> g.Developer).Where(entity => entity.Id == id).FirstOrDefaultAsync();
			TicketEdit detail = new TicketEdit
			{
				Id = ticket.Id,
				Title = ticket.Title,
				Description = ticket.Description,
				Priority = ticket.Priority,
				Status = ticket.Status,
				Type = ticket.Type,
				DeveloperName = ticket.Developer.UserName,
				
			};
			return detail;
		}
		public async Task<bool> EditTicketAsync(int id, TicketEdit detail)
		{
			TicketEntity ticketToEdit = await _context.Tickets.Include(g=>g.Developer).Where(entity=>entity.Id == id).FirstOrDefaultAsync();
			if (ticketToEdit.Title != detail.Title)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Title",
					OldValue = ticketToEdit.Title,
					NewValue = detail.Title,
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);

			}
			if(ticketToEdit.Description != detail.Description)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Description",
					OldValue = ticketToEdit.Description,
					NewValue = detail.Description,
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);
			}
			if(ticketToEdit.Developer.UserName != detail.DeveloperName)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Developer",
					OldValue = ticketToEdit.Developer.UserName,
					NewValue = detail.DeveloperName,
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);
			}
			if(ticketToEdit.Status != detail.Status)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Status",
					OldValue = ticketToEdit.Status.ToString(),
					NewValue = detail.Status.ToString(),
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);
			}
			if(ticketToEdit.Priority  != detail.Priority)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Priority",
					OldValue = ticketToEdit.Priority.ToString(),
					NewValue = detail.Priority.ToString(),
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);
			}
			if(ticketToEdit.Type != detail.Type)
			{
				TicketHistoryEntity newHistory = new TicketHistoryEntity
				{
					TicketId = id,
					Property = "Type",
					OldValue = ticketToEdit.Type.ToString(),
					NewValue = detail.Type.ToString(),
					ChangedDate = DateTime.Now

				};
				_context.TicketHistories.Add(newHistory);
			}
			await _context.SaveChangesAsync();
			ticketToEdit.Title = detail.Title;
			ticketToEdit.Description = detail.Description;
			ticketToEdit.Status = detail.Status;
			ticketToEdit.Priority = detail.Priority;
			ticketToEdit.Type = detail.Type;
			ApplicationUser newDeveloper = await _userManager.FindByNameAsync(detail.DeveloperName);
			ticketToEdit.DeveloperId = newDeveloper.Id;
			ticketToEdit.UpdatedDate = DateTime.Now;
			await _context.SaveChangesAsync();
			return true;

		}

		public async Task<TicketDetail> GetTicketDetailByIdAsync(int id, int? commentPage, int? historyPage)
		{
			int commentPageNumber = (commentPage ?? 1);
			int historyPageNumber = (historyPage ?? 1);

			TicketEntity ticket = await _context.Tickets
				.Include(g=>g.Developer)
				.Include(g=>g.Submitter)
				.Include(g=>g.Project)
				.Include(g=>g.TicketHistories)
				.Include(g=>g.TicketComments)
				.ThenInclude(comment => comment.Commenter)
				.Where(entity => entity.Id == id).FirstOrDefaultAsync();

			TicketDetail detail = new TicketDetail
			{
				Id = id,
				Title = ticket.Title,
				Description = ticket.Description,
				Status = ticket.Status,
				Priority = ticket.Priority,
				Type = ticket.Type,
				CreatedDate = ticket.CreatedDate,
				UpdatedDate = ticket.UpdatedDate,
				DeveloperName = ticket.Developer.UserName,
				SubmitterName = ticket.Submitter.UserName,
				ProjecName = ticket.Project.Title,
				TicketComments = ticket.TicketComments.OrderBy(c=>c.CreatedDate).ToPagedList(commentPageNumber,5),
				TicketHistories = ticket.TicketHistories.OrderBy(c=>c.ChangedDate).ToPagedList(historyPageNumber,5),
			};
			return detail;
		}

		public async Task<bool> CommentTicketAsync(TicketCommentCreate detail)
		{
			TicketCommentEntity comment = new TicketCommentEntity
			{
				TicketId = detail.TicketId,
				Message = detail.Message,
				CreatedDate = DateTime.Now,
				CommenterId = _userId
			};
			_context.TicketComments.Add(comment);
			int numOfChanges = await _context.SaveChangesAsync();
			return numOfChanges == 1;
		}

		public async Task<List<TicketStatusCount>> GetTicketByStatus_BarChartAsync()
		{
			List<TicketListDetail> tickets = await GetListOfAllTicketsAsync();

			List<TicketStatusCount> groupedTickets = tickets
									   .GroupBy(t => t.Status)
									   .Select(g => new TicketStatusCount { Status = g.Key, Count = g.Count() })
									   .ToList();
			return groupedTickets;
		}
		public async Task<List<TicketTypeCount>> GetTicketByType_BarChartAsync()
		{
			List<TicketListDetail> tickets = await GetListOfAllTicketsAsync();

			List<TicketTypeCount> groupedTickets = tickets
									   .GroupBy(t => t.Type)
									   .Select(g => new TicketTypeCount { Type = g.Key, Count = g.Count() })
									   .ToList();
			return groupedTickets;
		}
		public async Task<List<TicketPriorityCount>> GetTicketByPriority_BarChartAsync()
		{
			List<TicketListDetail> tickets = await GetListOfAllTicketsAsync();

			List<TicketPriorityCount> groupedTickets = tickets
									   .GroupBy(t => t.Priority)
									   .Select(g => new TicketPriorityCount { Priority = g.Key, Count = g.Count() })
									   .ToList();
			return groupedTickets;
		}
		public async Task<int> NumOfTicketsAsync()
		{
			List<TicketListDetail> tickets = await GetListOfAllTicketsAsync();
			return tickets.Count;
		}

	}

	
}
