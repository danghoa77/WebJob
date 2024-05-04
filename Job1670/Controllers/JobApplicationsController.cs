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

        public async Task<IActionResult> Apply()
        {
            int selectedJobId = 1; // Thay đổi giá trị này bằng cách lấy JobId từ form hoặc dữ liệu khác

            return RedirectToAction("CreateFromListing", "JobApplications", new { jobId = selectedJobId });
        }
        public async Task<IActionResult> CreateFromListing()
        {
            //ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName");
            //ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");
            //return View();
            var user = await _userManager.GetUserAsync(User);
            bool isJobSeeker = await _userManager.IsInRoleAsync(user, "JobSeeker");
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                ViewData["JobSeekerId"] = new List<SelectListItem>
                {
                     new SelectListItem { Text = user.UserName, Value = user.Id }
                };
            }
            else
            {
                var jobseeker = _context.JobSeekers.Select(e => new SelectListItem
                {
                    Text = e.FullName,
                    Value = e.JobSeekerId.ToString()
                }).ToList();

                ViewData["JobSeekerId"] = new SelectList(jobseeker, "Value", "Text");
            }
            ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");

            return View();
        }
        [HttpPost("CreateFromListing")]
        public async Task<IActionResult> CreateFromListing(int jobId)
        {
            // Logic để tạo SelectList cho JobSeekerId
            var user = await _userManager.GetUserAsync(User);
            bool isJobSeeker = await _userManager.IsInRoleAsync(user, "JobSeeker");
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                ViewData["JobSeekerId"] = new List<SelectListItem>
                {
                    new SelectListItem { Text = user.UserName, Value = user.Id }
                };
            }
            else
            {
                var jobseeker = _context.JobSeekers.Select(e => new SelectListItem
                {
                    Text = e.FullName,
                    Value = e.JobSeekerId.ToString()
                }).ToList();

                ViewData["JobSeekerId"] = new SelectList(jobseeker, "Value", "Text");
            }

            // Truyền JobId vào ViewData hoặc ViewBag
            ViewData["JobId"] = jobId;

            return View();
        }


        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.JobApplications.Include(j => j.JobSeeker).Include(j => j.Listing);
            //return View(await applicationDbContext.ToListAsync());
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

        // GET: JobApplications/Create
        public async Task<IActionResult> Create()
        {
            //ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName");
            //ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");
            //return View();
            var user = await _userManager.GetUserAsync(User);
            bool isJobSeeker = await _userManager.IsInRoleAsync(user, "JobSeeker");
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                ViewData["JobSeekerId"] = new List<SelectListItem>
                {
                     new SelectListItem { Text = user.UserName, Value = user.Id }
                };
            }
            else
            {
                var jobseeker = _context.JobSeekers.Select(e => new SelectListItem
                {
                    Text = e.FullName,
                    Value = e.JobSeekerId.ToString()
                }).ToList();

                ViewData["JobSeekerId"] = new SelectList(jobseeker, "Value", "Text");
            }
            ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title");

            return View();
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,JobSeekerId,CoverLetter,Status")] jobappModelBind jobapp)
        {
            if (!ModelState.IsValid)
            {
                ViewData["JobSeekerId"] = new SelectList(_context.JobSeekers, "JobSeekerId", "FullName", jobapp.JobSeekerId);
                ViewData["JobId"] = new SelectList(_context.Listings, "JobId", "Title", jobapp.JobId);
                return View(jobapp);

            }
            var jobApplication = new JobApplication
            {
                JobId = jobapp.JobId,
                JobSeekerId = jobapp.JobSeekerId,
                CoverLetter = jobapp.CoverLetter,
                Status = jobapp.Status,
            };
            _context.Add(jobApplication);
            await _context.SaveChangesAsync();
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
                return Problem("Entity set 'ApplicationDbContext.JobApplications'  is null.");
            }
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return (_context.JobApplications?.Any(e => e.JobApplicationId == id)).GetValueOrDefault();
        }
    }
}
