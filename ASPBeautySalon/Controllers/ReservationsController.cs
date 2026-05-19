using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPBeautySalon.Data;
using ASPBeautySalon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ASPBeautySalon.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Client> _userManager;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<Client> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Reservations
                .Include(r => r.Clients)
                .Include(r => r.Services);

            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                return View(await query.ToListAsync());

            return View(await query
                .Where(r => r.ClientId == userId)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["ServiceId"] =
                new SelectList(_context.Services, "Id", "Name");

            return View();
        }


        [HttpGet]
        public IActionResult GetTakenHours(DateTime date, int serviceId)
        {
            var takenHours = _context.Reservations
                .Where(r =>
                    r.DateReservation.Date == date.Date &&
                    r.ServiceId == serviceId)
                .Select(r => r.DateReservation.ToString("HH:mm"))
                .ToList();

            return Json(takenHours);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ServiceId,DateReservation")]
            Reservation reservation)
        {
            reservation.ClientId = _userManager.GetUserId(User);
            reservation.DateRegOn = DateTime.Now;

            if (reservation.DateReservation < DateTime.Now)
                ModelState.AddModelError("", "Минала дата не е позволена.");

            if (reservation.DateReservation.Minute != 0)
                ModelState.AddModelError("", "Само кръгъл час.");

            int hour = reservation.DateReservation.Hour;

            if (hour < 9 || hour > 20)
                ModelState.AddModelError("", "Работно време: 09:00 - 20:00");

            bool taken = await _context.Reservations.AnyAsync(r =>
                r.DateReservation == reservation.DateReservation &&
                r.ServiceId == reservation.ServiceId);

            if (taken)
                ModelState.AddModelError("", "Този час вече е зает.");

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ServiceId"] =
                new SelectList(_context.Services, "Id", "Name", reservation.ServiceId);

            return View(reservation);
        }

        //GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Clients)
                .Include(r => r.Services)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}