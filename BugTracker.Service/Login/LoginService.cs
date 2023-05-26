using BugTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using BugTracker.Data.Entities;
using BugTracker.Model.Login;
using System.Security.Claims;

namespace BugTracker.Service.Login
{
    public class LoginService : ILoginService
    {
        private ApplicationDbContext _context;
        // need to check program.cs?
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> CreateUserAsync(UserCreate model)
        {
            if(model.VerifyPassword != model.Password)
            {
                return null;
            }
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,

            };
            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return result;
            }

            return result;
        }
        public async Task<SignInResult> LoginAsync(UserLogin model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user != null)
            {
                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
					
					await _signInManager.SignInAsync(user, isPersistent: false);
                    return result;
				}
            }

            return null;
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            
        }
        
    }
}
