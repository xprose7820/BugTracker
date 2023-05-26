using BugTracker.Data.Entities;
using BugTracker.Model.Ticket;
using BugTracker.Model.TicketComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BugTracker.Service.Ticket
{
	public interface ITicketService
	{
		public Task<List<TicketListDetail>> GetListOfAllTicketsAsync();
		public Task<IPagedList<TicketListDetail>> GetListOfAllTicketsAsync(int? myTicketPage);
		public Task<bool> CreateTicketAsync(TicketCreate model);
		//uses ihttpcontext
		public Task<List<TicketListDetail>> GetListOfTicketsByDeveloperIdAsync();
		public Task<List<TicketListDetail>> GetListOfTicketsBySubmitterIdAsync();
		public Task<TicketEdit> GetTicketForEditByIdAsync(int id);
		public Task<bool> EditTicketAsync(int id, TicketEdit detail);
		public Task<TicketDetail> GetTicketDetailByIdAsync(int id, int? commentPage, int? historyPage);
		public Task<bool> CommentTicketAsync(TicketCommentCreate detail);
		public Task<List<TicketStatusCount>> GetTicketByStatus_BarChartAsync();
		public Task<List<TicketTypeCount>> GetTicketByType_BarChartAsync();
		public Task<List<TicketPriorityCount>> GetTicketByPriority_BarChartAsync();
		Task<int> NumOfTicketsAsync();
	}
}
