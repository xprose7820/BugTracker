using BugTracker.Model.Login;
using BugTracker.Service.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace BugTrackerMVC.Controllers
{


    //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
    public class LoginController : Controller
    {
        private ILoginService _service;
        public LoginController(ILoginService service)
        {
            _service = service;
        }
        [HttpGet]
        // should display Login form first, but the actual post will be called "Index(LoginDetail detail)" 

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _service.LoginAsync(model);
            if(result is null)
            {
				ModelState.AddModelError(string.Empty, "User/Password pair doesn't exist");
                return View(model);
            }
            if (result.Succeeded)
            {
                return RedirectToAction("BarChart","Ticket");
            }
			return View(model);

			

		}

        [HttpGet]
        // will display view of create account
        public async Task<IActionResult> Create()
        {
            return View();
        }
        // after going into view, this will actually do the creating of an account
        [HttpPost]
        public async Task<IActionResult> Create(UserCreate model)
        {
            if(!ModelState.IsValid)
            {
                
                return View(model);
            }
            IdentityResult result = await _service.CreateUserAsync(model);
            if(result is null)
            {
                ModelState.AddModelError(string.Empty, "Passwords don't match");
                return View(model);
            }
			if (result.Succeeded)
            {

				return RedirectToAction("Index", "Home");
			}
			else
			{
				// Add all errors to the ModelState to be displayed in the view
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
                return View(model);
			}
			
        }
		[HttpGet]
		public async Task<IActionResult> DemoLogin()
		{

            return View("~/Views/Login/Demo.cshtml");
		}
		[HttpGet]
        // need to make button for this 
        public async Task<IActionResult> SignOut()
        {
            await _service.SignOutAsync();
            return RedirectToAction("Index","Login");
        }
       

    }
}
