using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chaat.Data;
using System.Security.Claims;
using Chaat.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Chaat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            if (User.Identity.IsAuthenticated)
            {
                var aspUser = _context.Users.SingleOrDefault(u => u.Id == UserID());
                if (aspUser != null && aspUser.Activated)
                {
                    var user = _context.ChatUsers.SingleOrDefault(u => u.AspUserID == UserID());
                    user.friends = await _context.ChatUsers.ToListAsync<User>();
                    return View("Index", user);
                }
                else
                {
                    return RedirectToAction("NotActivated", "Account");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private string UserID()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }
    }
}
