using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Chaat.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool Activated { get; set; }

        [StringLength(10, ErrorMessage = "Kod składa się z 10 znaków.", MinimumLength = 10)]
        [Display(Name ="Kod aktywacyjny:")]
        public string Code { get; set; }
    }
}
