using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job1670.Data;
using Job1670.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Job1670.Controllers
{
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public JobApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        public class jobappModelBind
        {
            [Required]
            public int JobId { get; set; }

            [Required]
            public string JobSeekerId { get; set; }
            public string CoverLetter { get; set; }

            public string Status { get; set; }
        }
        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            bool isEmployer = await _userManager.IsInRoleAsync(user, "Employer");

            IQueryable<JobApplication> jobapp = _context.JobApplications
                .Include(a => a.Listing)
                .Include(a => a.JobSeeker);
            if (!isAdmin && !isEmployer)
            {
                jobapp = jobapp.Where(j => j.JobSeekerId == user.Id);
            }
            ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");
            return View(await jobapp.ToListAsync());
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobApplications == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .Include(j => j.JobSeeker)
                .Include(j => j.Listing)
                .FirstOrDefaultAsync(m => m.JobApplicationId == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        [HttpGet, ActionName("Create")]
        // GET: JobApplications/Create
        public async Task<IActionResult> Create()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            else { 
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            bool isJobSeeker = await _userManager.IsInRoleAsync(user, "JobSeeker");

            if (isAdmin)
            {
                ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");
                ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName");
            }
            else if (isJobSeeker)
            {
                var jobSeekerId = user.Id;
                var jobId = _context.JobApplications.FirstOrDefault(j => j.JobSeekerId == jobSeekerId)?.JobId;
                ViewData["JobId"] = new SelectList(new List<Listing> { _context.Listings.Find(jobId) }, "JobId", "Title");
                ViewData["JobSeekerId"] = new SelectList(new List<JobSeeker> { _context.JobSeekers.Find(jobSeekerId) }, "JobSeekerId", "FullName");
            }
           
            return View();
            }
        }


        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("JobId, JobSeekerId,CoverLetter,Status")] jobappModelBind jobapp)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(c => c.JobId == id && c.Status == "operating");
            if (listing == null)
            {
                TempData["failed"] = "Cannot create job application because the listing is not in operating status.";
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName", jobapp.JobSeekerId);
                ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title", jobapp.JobId);
                TempData["failed"] = "Unsuccessfull";
                return View(jobapp);

            }
            var jobApplication = new JobApplication
            {
                JobId = id,
                JobSeekerId = jobapp.JobSeekerId,
                CoverLetter = jobapp.CoverLetter,
                Status = jobapp.Status,
            };
            _context.Add(jobApplication);
            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }

        // GET: JobApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobApplications == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName", jobApplication.JobSeekerId);
            ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title", jobApplication.JobId);
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,JobSeekerId,CoverLetter,Status")] jobappModelBind jobapp)
        {
            if (!ModelState.IsValid)
            {
                ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName", jobapp.JobSeekerId);
                ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title", jobapp.JobId);
                TempData["failed"] = "Unsuccessfull";
                return View(jobapp);
            }

            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            jobApplication.JobId = jobapp.JobId;
            jobApplication.JobSeekerId = jobapp.JobSeekerId;
            jobApplication.Status = jobapp.Status;
            jobApplication.CoverLetter = jobapp.CoverLetter;

            _context.Update(jobApplication);
            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobApplications == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .Include(j => j.JobSeeker)
                .Include(j => j.Listing)
                .FirstOrDefaultAsync(m => m.JobApplicationId == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobApplications == null)
            {
                TempData["failed"] = "Unsuccessfull";
                return Problem("Entity set 'ApplicationDbContext.JobApplications'  is null.");
            }
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Successful.";
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return (_context.JobApplications?.Any(e => e.JobApplicationId == id)).GetValueOrDefault();
        }
    }
}
