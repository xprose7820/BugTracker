using System.ComponentModel.DataAnnotations;

namespace BugTracker.Model.Login
{
	public class UserCreate
	{
		[Required]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string Email { get; set; }
		public string VerifyPassword { get; set; }
		

	}
}
