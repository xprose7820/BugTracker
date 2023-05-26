using BugTracker.Model.Login;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Service.Login
{
    public interface ILoginService
    {
        Task<IdentityResult> CreateUserAsync(UserCreate model);
        Task<SignInResult> LoginAsync(UserLogin model);
        Task SignOutAsync();
        
    }
}
