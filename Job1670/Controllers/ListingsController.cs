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
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ListingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                ViewBag.ErrorMessage = "Please enter Valid.";
                return View(new List<Listing>());
            }

            var listings = _context.Listings
                                .Where(s => s.Title.Contains(searchTerm))
                                .ToList();

            if (!listings.Any())
            {
                return View("Search", listings);
            }
            return View("Search", listings);
        }
        // GET: Listings
        public async Task<IActionResult> Index()
        {
            
            if (!_signInManager.IsSignedIn(User))
            {
                var applicationDbContext = _context.Listings.Include(l => l.Category).Include(l => l.Employer);
                return View(await applicationDbContext.ToListAsync());
            }
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            bool isJobSeeker = await _userManager.IsInRoleAsync(user, "JobSeeker");

            IQueryable<Listing> listings = _context.Listings
                                          .Include(l => l.Employer)
                                          .Include(l => l.Category);

            if (!isAdmin && !isJobSeeker)
            {
                listings = listings.Where(l => l.EmployerId == user.Id);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
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
        [HttpGet("Listings/Create")]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isEmployer = await _userManager.IsInRoleAsync(user, "Employer");
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            return View();
        }


        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Listings/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployerId,CategoryId,Title,Deadline,Description,Status")] listModelBing listing)
        {
            if (!ModelState.IsValid|| !_context.Categories.Any(c => c.CategoryId == listing.CategoryId && c.Status == "operating"))
            {
                TempData["failed"] = "New jobs cannot be created with this category.";
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "EmployerId", listing.EmployerId);
                return RedirectToAction(nameof(Index));
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
            TempData["success"] = "Create successful jobs.";
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
                TempData["failed"] = "Unsuccessfull";
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
            TempData["success"] = "Successful.";
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
                TempData["failed"] = "Unsuccessfull";
                return Problem("Entity set 'ApplicationDbContext.Listings'  is null.");
            }
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }

        private bool ListingExists(int id)
        {
            return (_context.Listings?.Any(e => e.JobId == id)).GetValueOrDefault();
        }
    }
}
