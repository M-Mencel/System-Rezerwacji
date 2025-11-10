using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System_Rezerwacji.Data;
using System_Rezerwacji.Models;

namespace System_Rezerwacji.Controllers
{
    /*[Authorize(Roles = "Admin")]*/
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult AdminIndex()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
                return NotFound($"Użytkownik nie został znaleziony.");



            var userRoles = new List<UserDetails>();

            foreach (var user in users)
            {
                userRoles.Add(new UserDetails
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(userRoles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserRoles
            {
                User = user,
                UsersRoles = (await _userManager.GetRolesAsync(user)).ToList(),
                AllRoles = _roleManager.Roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            var addToRole = selectedRoles.Except(currentRoles);
            await _userManager.AddToRolesAsync(user, addToRole);

            var deleteRole = currentRoles.Except(selectedRoles);
            await _userManager.RemoveFromRolesAsync(user, deleteRole);

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var deleteUser = await _userManager.DeleteAsync(user);

            if(deleteUser.Succeeded)
            {
                return RedirectToAction("Users");
            }

            else
            {
                foreach(var error in deleteUser.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Dashboard()
        {
            var Today = DateTime.Today;
            var startOfTheWeek = Today.AddDays(-(int)Today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfTheWeek.AddDays(7);

            var todayReservationAmount = _context.Reservations
                .Count(r => r.ReservationDate >= Today && r.ReservationDate > Today.AddDays(1));

            var weeklyReservationAmount = _context.Reservations
                .Count(r => r.ReservationDate >= startOfTheWeek && r.ReservationDate < endOfWeek);

            var weeklyStats = _context.Reservations
                .Where(r => r.ReservationDate >= startOfTheWeek && r.ReservationDate < endOfWeek)
                .GroupBy(r => r.ReservationDate)
                .Select(g => new
                {
                    Day = g.Key,
                    Count = g.Count()
                })
            .OrderBy(x => x.Day)
            .ToList();

            var registeredCustomers = _context.Customers.Count();
            var activeCustomers = _context.Customers
                .Count(r => r.Reservations.Any());

            var topServices = _context.Reservations
                .GroupBy(r => r.ServiceID)
                .Select(g => new
                {
                    ServiceId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .Join(_context.Services,
                      g => g.ServiceId,
                      s => s.ServiceID,
                      (g, s) => new { s.Name, g.Count })
                .ToList();


            ViewBag.TodayReservation = todayReservationAmount;
            ViewBag.WeeklyReservation = weeklyReservationAmount;
            ViewBag.WeeklyReservationByDay = weeklyStats;
            ViewBag.RegisteredCustomers = registeredCustomers;
            ViewBag.ActiveCustomers = activeCustomers;
            ViewBag.TopThreeServices = topServices;

            return View();
        }

    }
}
