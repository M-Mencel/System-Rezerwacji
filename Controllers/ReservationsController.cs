using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System_Rezerwacji.Data;
/*using System_Rezerwacji.Data.Migrations;*/
using System_Rezerwacji.Models;

namespace System_Rezerwacji.Controllers
{
    [Authorize(Roles = "Customer,Admin,Employee")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            var applicationDbContext = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .Where(r => r.Customer.Email == userEmail);



            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Manage(string sortOrder)
        {
            var userEmail = User.Identity?.Name;
            var Reservations = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .AsQueryable();



            ViewBag.ServiceNameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.CustomerSort = sortOrder == "customer_asc" ? "customer_desc" : "customer_asc";
            ViewBag.DateSortParm = sortOrder == "ReservationDate" ? "date_desc" : "ReservationDate";

            switch (sortOrder)
            {
                case "date_desc":
                    Reservations = Reservations.OrderByDescending(r => r.ReservationDate);
                    break;
                case "ReservationDate":
                    Reservations = Reservations.OrderBy(r => r.ReservationDate);
                    break;
                case "customer_desc":
                    Reservations = Reservations.OrderByDescending(r => r.Customer.FullName);
                    break;
                case "customer_asc":
                    Reservations = Reservations.OrderBy(r => r.Customer.FullName);
                    break;
                case "name_desc":
                    Reservations = Reservations.OrderByDescending(s => s.Service.Name);
                    break;
                case "name_asc":
                    Reservations = Reservations.OrderBy(s => s.Service.Name);
                    break;
                default:
                    Reservations = Reservations.OrderBy(r => r.ReservationDate);
                    break;
            }


            return View(await Reservations.AsNoTracking().ToListAsync());
        }
        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name");
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationID,ReservationDate,CustomerID,ServiceID")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Identity?.Name;
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == userEmail);

                if (customer == null)
                {
                    return RedirectToAction("Create", "Customers");
                }


                var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceID == reservation.ServiceID);
                if (service == null) return NotFound();

                double durationInMinutes = service.Duration.TotalMinutes;

                var start = reservation.ReservationDate;
                var end = start.Add(service.Duration);

                var existingReservations = await _context.Reservations
                    .Include(r => r.Service)
                    .Where(r => r.ServiceID == reservation.ServiceID &&
                            r.ReservationDate.Date == reservation.ReservationDate.Date)
                    .ToListAsync();



                var overlap = existingReservations.Any(r =>
                {
                    var existingStart = r.ReservationDate;
                    var existingEnd = existingStart.Add(r.Service.Duration);
                    return existingStart < end && existingEnd > start;
                });


                DateTime FindNextAvailableSlot(DateTime requestedStart, TimeSpan duration, List<Reservation> existing)
                {
                    var sorted = existing
                        .OrderBy(r => r.ReservationDate)
                        .ToList();

                    DateTime cursor = requestedStart;

                    foreach (var r in sorted)
                    {
                        var rStart = r.ReservationDate;
                        var rEnd = rStart.Add(r.Service.Duration);

                        if (cursor < rStart)
                        {
                            if (cursor.Add(duration) <= rStart)
                                return cursor; 
                        }

                        
                        if (cursor < rEnd)
                            cursor = rEnd;
                    }

                    return cursor; 
                }




                if (overlap)
                {
                    var nextAvailable = FindNextAvailableSlot(start, service.Duration, existingReservations);

                    ModelState.AddModelError(string.Empty, $"Termin zajęty. Najbliższy wolny to: {nextAvailable.ToString("g")}");
                    ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name", reservation.ServiceID);
                    return View(reservation);
                }




                reservation.CustomerID = customer.CustomerID;

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", reservation.CustomerID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "Name", reservation.ServiceID);
            return View(reservation);
        }

        public async Task<IActionResult> MyReservations()
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Reservations
                .FirstOrDefaultAsync(r => r.CustomerID == r.ReservationID);

            return View(customer);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", reservation.CustomerID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", reservation.ServiceID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationID,ReservationDate,CustomerID,ServiceID")] Reservation reservation)
        {
            if (id != reservation.ReservationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "CustomerID", reservation.CustomerID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", reservation.ServiceID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationID == id);
        }
    }
}
