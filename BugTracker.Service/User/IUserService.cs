using BugTracker.Model.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BugTracker.Service.User
{
    public interface IUserService
	{
		Task<List<UserListDetail>> GetListOfAllUsersAsync();
		Task<IPagedList<UserListDetail>> GetListOfAllUsersAsync(int? userPage);
		Task<List<RoleListDetail>> GetListOfAllRolesAsync();
		Task<bool> AssignUserRoleAsync(UserRoleUpdate model);
		Task<List<ProjectManagerListDetail>> GetListOfAllUnasssignedProjectManagersAsync();
		Task<List<UserListDetail_Projects>> GetListOfAllUsers_ProjectsAsync();
		Task<IPagedList<UserListDetail_Projects>> GetListOfAllUsers_ProjectsAsync(int? projectPage);

        Task<List<UserListDetail_Basic>> GetListOfAllDevelopersAsync();
		Task<List<UserListDetail_Basic>> GetListOfAllSubmittersAsync();
		Task<int> NumOfUsersAsync();
	}
}
