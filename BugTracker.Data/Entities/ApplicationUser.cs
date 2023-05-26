using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public virtual List<UserProjectEntity> UserProjects { get; set; }
        public virtual List<TicketEntity> Ticket { get; set; }
    }

}
