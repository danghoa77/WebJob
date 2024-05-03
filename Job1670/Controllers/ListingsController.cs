using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job1670.Data;
using Job1670.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Job1670.Controllers
{
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ListingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public class listModelBing
        {

            [Required]
            public string EmployerId { get; set; }

            [Required]
            public int CategoryId { get; set; }

            public string Title { get; set; }

            public DateTime Deadline { get; set; }

            public string Description { get; set; }

            public string Status { get; set; }
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            IQueryable<Listing> listings;

            if (isAdmin)
            {
                listings = _context.Listings.Include(e => e.Employer);
            }
            else
            {
                listings = _context.Listings.Include(e => e.Employer)
                                             .Where(e => e.EmployerId == user.Id);
            }

            return View(await listings.ToListAsync());
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Employer)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Listings/Create
        public async Task<IActionResult> Create1()//lỗi viewbag
        {
            var user = await _userManager.GetUserAsync(User);
            bool isEmployer = await _userManager.IsInRoleAsync(user, "Employer");

            if (isEmployer)
            {
                ViewData["EmployerId"] = new SelectListItem { Text = user.Id, Value = user.Id };
            }
            else
            {
                var employers = _context.Employers.Select(e => new SelectListItem { Text = e.CompanyName, Value = e.EmployerId });
                ViewData["EmployerId"] = new SelectList(employers, "Value", "Text");
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            return View();
        }
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create()// đã sửa 
        {
            var user = await _userManager.GetUserAsync(User);
            bool isEmployer = await _userManager.IsInRoleAsync(user, "Employer");

            if (isEmployer)
            {
                ViewData["EmployerId"] = new List<SelectListItem>
                {
                     new SelectListItem { Text = user.UserName, Value = user.Id }
                };
            }
            else
            {
                var employers = _context.Employers.Select(e => new SelectListItem
                {
                    Text = e.CompanyName,
                    Value = e.EmployerId.ToString()
                }).ToList();

                ViewData["EmployerId"] = new SelectList(employers, "Value", "Text");
            }

            // Populate categories dropdown
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            return View();
        }


        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployerId,CategoryId,Title,Deadline,Description,Status")] listModelBing listing)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", listing.EmployerId);
                return View(listing);
            }
            var list = new Listing
            {
                EmployerId = listing.EmployerId,
                CategoryId = listing.CategoryId,
                Title = listing.Title,
                Deadline = listing.Deadline,
                Description = listing.Description,
                Status = listing.Status,
            };
            _context.Add(list);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Employer")]
        // GET: Listings/Edit/5
        [HttpGet("Listings/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            bool isEmployer = await _userManager.IsInRoleAsync(user, "Employer");

            if (isEmployer)
            {
                ViewData["EmployerId"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = user.Id, Value = user.Id }
                    }, "Value", "Text", listing.EmployerId);
            }
            else
            {
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", listing.EmployerId);
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
            return View(listing);
        }


        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Listings/Edit/5
        [HttpPost("Listings/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployerId,CategoryId,Title,Deadline,Description,Status")] listModelBing model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", model.CategoryId);
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName", model.EmployerId);
                return View(model);
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }

            // Cập nhật các trường của listing dựa trên model
            listing.EmployerId = model.EmployerId;
            listing.CategoryId = model.CategoryId;
            listing.Title = model.Title;
            listing.Deadline = model.Deadline;
            listing.Description = model.Description;
            listing.Status = model.Status;

            _context.Update(listing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles ="Admin")]
        // GET: Listings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Employer)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Listings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Listings'  is null.");
            }
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingExists(int id)
        {
            return (_context.Listings?.Any(e => e.JobId == id)).GetValueOrDefault();
        }
    }
}
