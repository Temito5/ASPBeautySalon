//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using ASPBeautySalon.Data;
//using ASPBeautySalon.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authorization;

//namespace ASPBeautySalon.Controllers
//{
//    [Authorize]//изискващо Login
//    public class ReservationsController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<Client> _userManager;//за достъп на логнатия User

//        public ReservationsController(ApplicationDbContext context, UserManager<Client> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        GET: Reservations
//        public async Task<IActionResult> Index()
//        {
//            if (User.IsInRole("Admin"))
//            {
//                var applicationDbContext = _context.Reservations.Include(r => r.Clients).Include(r => r.Services);
//                return View(await applicationDbContext.ToListAsync());
//            }
//            else
//            {
//                var applicationDbContext = _context.Reservations.
//                    Include(r => r.Clients).
//                    Include(r => r.Services).
//                    Where(x => x.ClientId == _userManager.GetUserId(User));
//                return View(await applicationDbContext.ToListAsync());
//            }
//        }

//        GET: Reservations/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var reservation = await _context.Reservations
//                .Include(r => r.Clients)
//                .Include(r => r.Services)
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (reservation == null)
//            {
//                return NotFound();
//            }

//            return View(reservation);
//        }

//        GET: Reservations/Create
//        public IActionResult Create()
//        {
//            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id");
//            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
//            return View();
//        }

//        POST: Reservations/Create
//        To protect from overposting attacks, enable the specific properties you want to bind to.
//        For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

//       [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("ServiceId,DateReservation")] Reservation reservation)
//        {
//            SuperAdmin да няма право да резервира
//            if (User.IsInRole("SuperAdmin"))
//            {
//                return RedirectToAction(nameof(Index));
//            }

//            reservation.DateRegOn = DateTime.Now;
//            reservation.ClientId = _userManager.GetUserId(User);

//            Минала дата
//            if (reservation.DateReservation < DateTime.Now)
//            {
//                ModelState.AddModelError("", "Не може да резервирате минала дата.");
//            }

//            Само кръгъл час
//            if (reservation.DateReservation.Minute != 0)
//            {
//                ModelState.AddModelError("", "Резервацията трябва да бъде на кръгъл час.");
//            }

//            Работно време
//            int hour = reservation.DateReservation.Hour;

//            if (hour < 9 || hour >= 18)
//            {
//                ModelState.AddModelError("", "Работното време е от 09:00 до 18:00.");
//            }

//            Проверка за зает час
//            bool taken = _context.Reservations.Any(r =>
//                r.DateReservation == reservation.DateReservation);

//            if (taken)
//            {
//                ModelState.AddModelError("", "Този час вече е зает.");
//            }

//            if (ModelState.IsValid)
//            {
//                _context.Reservations.Add(reservation);
//                await _context.SaveChangesAsync();

//                return RedirectToAction(nameof(Index));
//            }

//            ViewData["ServiceId"] =
//                new SelectList(_context.Services, "Id", "Name", reservation.ServiceId);

//            return View(reservation);
//        }

//        GET: Reservations/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var reservation = await _context.Reservations.FindAsync(id);
//            if (reservation == null)
//            {
//                return NotFound();
//            }
//            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", reservation.ClientId);
//            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", reservation.ServiceId);
//            return View(reservation);
//        }

//        POST: Reservations/Edit/5
//         To protect from overposting attacks, enable the specific properties you want to bind to.
//         For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceId,ClientId,DateReservation,DateRegOn")] Reservation reservation)
//        {
//            reservation.DateRegOn = DateTime.Now;
//            if (id != reservation.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(reservation);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!ReservationExists(reservation.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", reservation.ClientId);
//            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", reservation.ServiceId);
//            return View(reservation);
//        }

//        GET: Reservations/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var reservation = await _context.Reservations
//                .Include(r => r.Clients)
//                .Include(r => r.Services)
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (reservation == null)
//            {
//                return NotFound();
//            }

//            return View(reservation);
//        }

//        POST: Reservations/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var reservation = await _context.Reservations.FindAsync(id);
//            if (reservation != null)
//            {
//                _context.Reservations.Remove(reservation);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool ReservationExists(int id)
//        {
//            return _context.Reservations.Any(e => e.Id == id);
//        }
//    }
//}
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

        // =========================
        // AJAX TAKEN HOURS
        // =========================

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

        // =========================
        // CREATE
        // =========================

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

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}