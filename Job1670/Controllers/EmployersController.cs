using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job1670.Data;
using Job1670.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Job1670.Controllers
{
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public class employerModelBind
        {
            [Required]
            public string CompanyName { get; set; }

            public string Address { get; set; }

            public string? Detail { get; set; }
            public string Phone { get; set; }

            public string Email { get; set; }

            public string ApplicationUserId { get; set; }

        }
        [Authorize(Roles = "Admin, Employer")]
        // GET: Employers
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            IQueryable<Employer> applicationDbContext;

            if (isAdmin)
            {
                applicationDbContext = _context.Employers.Include(e => e.ApplicationUser);
            }
            else
            {
                applicationDbContext = _context.Employers.Include(e => e.ApplicationUser)
                                                          .Where(e => e.ApplicationUser.Id == user.Id);
            }

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Employers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployerId == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }
        [Authorize(Roles = "Master")]

        // GET: Employers/Create
        public IActionResult Create()
        {
            ViewData["EmployerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyName,Address,Detail,Phone,Email,ApplicationUserId")] employerModelBind employer)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = employer.Email, Email = employer.Email };
                var result = await _userManager.CreateAsync(user, "DefaultPassword@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employer");

                    var emp = new Employer
                    {
                        EmployerId = user.Id,
                        ApplicationUserId = user.Id,
                        CompanyName = employer.CompanyName,
                        Phone = employer.Phone,
                        Address = employer.Address,
                        Detail = employer.Detail,
                        Email = employer.Email,
                    };

                    _context.Add(emp);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Successful.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", employer.ApplicationUserId);
            TempData["failed"] = "Unsuccessfull";
            return View(employer);
        }
        [Authorize(Roles = "Admin, Employer")]
        [AcceptVerbs("GET", "POST", Route = "Employer/Edit/{id}")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers.FindAsync(id.ToString());
            if (employer == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", employer.ApplicationUserId);
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("CompanyName,Address,Detail,Phone,Email,ApplicationUserId")] employerModelBind employer)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", employer.ApplicationUserId);
                TempData["failed"] = "Unsuccessfull";
                return View(employer);
            }
            var emp = await _context.Employers.FindAsync(id.ToString());
            if (emp == null)
            {
                return NotFound();
            }
            emp.CompanyName = employer.CompanyName;
            emp.Address = employer.Address;
            emp.Detail = employer.Detail;
            emp.Phone = employer.Phone;
            emp.Email = employer.Email;
            emp.ApplicationUserId = employer.ApplicationUserId;

            _context.Update(emp);
            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        // GET: Employers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployerId == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Employers == null)
            {
                TempData["failed"] = "Unsuccessfull";
                return Problem("Entity set 'ApplicationDbContext.Employers'  is null.");
            }
            var employer = await _context.Employers.FindAsync(id);
            var user = await _userManager.FindByIdAsync(employer.ApplicationUserId);
            if (employer != null)
            {
                await _userManager.DeleteAsync(user);
                _context.Employers.Remove(employer);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(string id)
        {
            return (_context.Employers?.Any(e => e.EmployerId == id)).GetValueOrDefault();
        }
    }
}
