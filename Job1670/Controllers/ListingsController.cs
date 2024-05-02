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
            var applicationDbContext = _context.Listings.Include(l => l.Category).Include(l => l.Employer);
            return View(await applicationDbContext.ToListAsync());
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
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEmployer = await _userManager.IsInRoleAsync(currentUser, "Employer");
            if (isEmployer)
            {
                ViewData["EmployerId"] = new SelectList(new List<ApplicationUser> { currentUser }, "Id", "CompanyName", currentUser.Id);
            }
            else
            {
                ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,EmployerId,CategoryId,Title,Deadline,Description,Status")] listModelBing listing)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
            //    ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName", listing.EmployerId);
            //    return View(listing);
            //}
            //var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == listing.CategoryId);
            //if (category == null|| category.Status != "operating")
            //{
            //    TempData["failed"] = "Category created failed";
            //    ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(x => x.CategoryName), "CategoryId", "Name", listing.CategoryId);
            //    ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(x => x.CompanyName), "EmployerId", "Name", listing.EmployerId);
            //    return View(listing);
            //}
            //var list = new Listing
            //{
            //    EmployerId= listing.EmployerId,
            //    CategoryId= listing.CategoryId,
            //    Title=listing.Title,
            //    Deadline=listing.Deadline,
            //    Description=listing.Description,
            //    Status=listing.Status,
            //};
            //_context.Add(list);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            //var currentUser = await _userManager.GetUserAsync(User);

            //// Kiểm tra xem người dùng có vai trò là Employer hay không
            //bool isEmployer = await _userManager.IsInRoleAsync(currentUser, "Employer");

            //// Nếu là Employer, sử dụng EmployerId của người dùng hiện tại
            //if (isEmployer)
            //{
            //    listing.EmployerId = currentUser.Id;
            //}

            //if (!ModelState.IsValid)
            //{
            //    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
            //    ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName", listing.EmployerId);
            //    return View(listing);
            //}

            //var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == listing.CategoryId);
            //if (category == null || category.Status != "operating")
            //{
            //    TempData["failed"] = "Category creation failed";
            //    ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(x => x.CategoryName), "CategoryId", "Name", listing.CategoryId);
            //    ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(x => x.CompanyName), "EmployerId", "Name", listing.EmployerId);
            //    return View(listing);
            //}

            //var list = new Listing
            //{
            //    EmployerId = listing.EmployerId,
            //    CategoryId = listing.CategoryId,
            //    Title = listing.Title,
            //    Deadline = listing.Deadline,
            //    Description = listing.Description,
            //    Status = listing.Status,
            //};
            //_context.Add(list);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Listings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName", listing.EmployerId);
            return View(listing);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,EmployerId,CategoryId,Title,Deadline,Description,Status")] Listing listing)
        {
            if (id != listing.JobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingExists(listing.JobId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", listing.CategoryId);
            ViewData["EmployerId"] = new SelectList(_context.Employers, "EmployerId", "CompanyName", listing.EmployerId);
            return View(listing);
        }

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
