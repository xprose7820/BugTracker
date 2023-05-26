using BugTracker.Data.Entities;
using BugTracker.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugTracker.Model.User;
using BugTracker.Service.Project;
using BugTracker.Model.Project;
using X.PagedList;

namespace BugTracker.Service.User
{
	public class UserService : IUserService
	{
		private ApplicationDbContext _context;
		// need to check program.cs?
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private IProjectService _projectService;
		public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IProjectService projectService)
		{
			_projectService = projectService;
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public async Task<List<UserListDetail>> GetListOfAllUsersAsync()
		{
			List<ApplicationUser> users = await _userManager.Users.ToListAsync();
			List<UserListDetail> details = new List<UserListDetail>();
			foreach (ApplicationUser user in users)
			{
				if (user.Id == 0)
				{
					continue;
				}
				var userRoles = await _userManager.GetRolesAsync(user);
				var firstRole = userRoles.FirstOrDefault() ?? "N/A";

				details.Add(new UserListDetail
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					Role = firstRole
				});
			}
			return details;

		}
		public async Task<IPagedList<UserListDetail>> GetListOfAllUsersAsync(int? userPage)
		{
			int userPageNumber = (userPage ?? 1);
			List<ApplicationUser> users = await _userManager.Users.ToListAsync();
			List<UserListDetail> details = new List<UserListDetail>();
			foreach (ApplicationUser user in users)
			{
				if (user.Id == 0)
				{
					continue;
				}
				var userRoles = await _userManager.GetRolesAsync(user);
				var firstRole = userRoles.FirstOrDefault() ?? "N/A";

				details.Add(new UserListDetail
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					Role = firstRole
				});
			}
			IPagedList<UserListDetail> detailsPaged = details.OrderByDescending(x => x.Id).ToPagedList(userPageNumber,10);
			return detailsPaged;

		}

		public async Task<List<RoleListDetail>> GetListOfAllRolesAsync()
		{
			List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();
			List<RoleListDetail> details = new List<RoleListDetail>();
			foreach (ApplicationRole role in roles)
			{
				details.Add(new RoleListDetail
				{
					Id = role.Id,
					Name = role.Name
				});
			}
			return details;
		}
		public async Task<bool> AssignUserRoleAsync(UserRoleUpdate model)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(g => g.Id == model.UserId);

			var existingRole = await _userManager.GetRolesAsync(user);

			// Check if the user already has a role
			if (existingRole.Any())
			{
				// Remove the existing role
				var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRole);
				if (!removeResult.Succeeded)
				{
					return false; // Failed to remove the existing role
				}
			}
			var role = await _roleManager.Roles.FirstOrDefaultAsync(g => g.Id == model.RoleId);
			var result = await _userManager.AddToRoleAsync(user, role.Name);
			if (result.Succeeded)
			{
				return true;
			}
			return false;
		}
		public async Task<List<ProjectManagerListDetail>> GetListOfAllUnasssignedProjectManagersAsync()
		{
			List<int> assignedManagerId = await _context.Projects.Select(entity => entity.ProjectManagerId).ToListAsync();

			var users = await _userManager.GetUsersInRoleAsync("Project_Manager");
			List<ApplicationUser> unassignedManagers = users.Where(user => !assignedManagerId.Contains(user.Id)).ToList();

			List<ProjectManagerListDetail> details = unassignedManagers.Select(user => new ProjectManagerListDetail
			{
				Id = user.Id,
				UserName = user.UserName
			}).ToList();
			return details;
		}
		public async Task<List<UserListDetail_Projects>> GetListOfAllUsers_ProjectsAsync()
		{
			List<ApplicationUser> users = await _context.Users.ToListAsync();
			List<UserListDetail_Projects> details = new List<UserListDetail_Projects>();
			foreach (ApplicationUser user in users)
			{
				if (user.Id == 0)
				{
					continue;
				}
				var userRoles = await _userManager.GetRolesAsync(user);
				var firstRole = userRoles.FirstOrDefault() ?? "N/A";
				var projects = await _projectService.GetAllProjectsByUserIdAsync(user.Id);
				details.Add(new UserListDetail_Projects
				{

					Id = user.Id,
					UserName = user.UserName,
					Role = firstRole,
					Projects = projects ?? new List<ProjectListDetail>()

				});


			}
			return details;
		}
        public async Task<IPagedList<UserListDetail_Projects>> GetListOfAllUsers_ProjectsAsync(int? projectPage)
        {
			int projectPageNumber = (projectPage ?? 1);

            List<ApplicationUser> users = await _context.Users.ToListAsync();
            List<UserListDetail_Projects> details = new List<UserListDetail_Projects>();
            foreach (ApplicationUser user in users)
            {
                if (user.Id == 0)
                {
                    continue;
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var firstRole = userRoles.FirstOrDefault() ?? "N/A";
                var projects = await _projectService.GetAllProjectsByUserIdAsync(user.Id);
                details.Add(new UserListDetail_Projects
                {

                    Id = user.Id,
                    UserName = user.UserName,
                    Role = firstRole,
                    Projects = projects ?? new List<ProjectListDetail>()

                });


            }
			IPagedList<UserListDetail_Projects> detailsPaged = details.OrderByDescending(x => x.Id).ToPagedList(projectPageNumber, 5);
            return detailsPaged;
        }


        public async Task<List<UserListDetail_Basic>> GetListOfAllDevelopersAsync()
		{
			List<ApplicationUser> developers = await _userManager.Users.ToListAsync();
			List<UserListDetail_Basic> details = new List<UserListDetail_Basic>();
			foreach (ApplicationUser developer in developers)
			{
				if (await _userManager.IsInRoleAsync(developer, "Developer") || await _userManager.IsInRoleAsync(developer, "Demo_Developer"))
				{
					details.Add(new UserListDetail_Basic
					{
						Id = developer.Id,
						UserName = developer.UserName
					});
				}
			}
			return details;
		}
		public async Task<List<UserListDetail_Basic>> GetListOfAllSubmittersAsync()
		{
			List<ApplicationUser> submitters = await _userManager.Users.ToListAsync();
			List<UserListDetail_Basic> details = new List<UserListDetail_Basic>();
			foreach (ApplicationUser submitter in submitters)
			{
				if (await _userManager.IsInRoleAsync(submitter, "Submitter") || await _userManager.IsInRoleAsync(submitter, "Demo_Submitter"))
				{
					details.Add(new UserListDetail_Basic
					{
						Id = submitter.Id,
						UserName = submitter.UserName
					});
				}
			}
			return details;
		}

		public async Task<int> NumOfUsersAsync()
		{
			List<ApplicationUser> users = await _userManager.Users.ToListAsync();
			return users.Count;
		}

	}

}
