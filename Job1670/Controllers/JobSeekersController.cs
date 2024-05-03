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
    public class JobSeekersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public class jobseekerModelBind
        {           
            [Required]
            public string FullName { get; set; }

            public string Phone { get; set; }

            public string Address { get; set; }

            public string? Detail { get; set; }
            public string Email { get; set; }

            public string? CV { get; set; }

            public string ApplicationUserId { get; set; }
        }
        [Authorize(Roles = "Admin, JobSeeker")]
        // GET: JobSeekers
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "admin");
            IQueryable<JobSeeker> applicationDbContext;

            if (isAdmin)
            {
                applicationDbContext = _context.JobSeekers.Include(j => j.ApplicationUser);
            }
            else
            {
                applicationDbContext = _context.JobSeekers.Include(j => j.ApplicationUser)
                                                            .Where(j => j.ApplicationUser.Id == user.Id);
            }

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: JobSeekers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.JobSeekers == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeekers
                .Include(j => j.ApplicationUser)
                .FirstOrDefaultAsync(m => m.JobSeekerId == id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }
        [Authorize(Roles = "Admin")]
        // GET: JobSeekers/Create
        public IActionResult Create()
        {

            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["JobSeekerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: JobSeekers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(" FullName,Phone,Address,Detail,Email,CV,ApplicationUserId")] jobseekerModelBind jobSeekerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = jobSeekerModel.Email, Email = jobSeekerModel.Email };
                var result = await _userManager.CreateAsync(user, "DefaultPassword@123"); 

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "JobSeeker");

                    var jobSeeker = new JobSeeker
                    {
                        JobSeekerId = user.Id, 
                        ApplicationUserId = user.Id,
                        FullName = jobSeekerModel.FullName,
                        Phone = jobSeekerModel.Phone,
                        Address = jobSeekerModel.Address,
                        Detail = jobSeekerModel.Detail,
                        Email = jobSeekerModel.Email,
                        CV = jobSeekerModel.CV
                    };

                    _context.Add(jobSeeker);
                    await _context.SaveChangesAsync();
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobSeekerModel.ApplicationUserId);
            return View(jobSeekerModel);
        }

        [Authorize(Roles = "Admin, JobSeeker")]
        // GET: JobSeekers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobSeekers == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeekers.FindAsync(id.ToString());
            if (jobSeeker == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobSeeker.ApplicationUserId);
            return View(jobSeeker);
        }

        // POST: JobSeekers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FullName,Phone,Address,Detail,Email,CV,ApplicationUserId")] jobseekerModelBind jobSeeker)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobSeeker.ApplicationUserId);
                return View(jobSeeker);
            }
            var jobs = await _context.JobSeekers.FindAsync(id.ToString());
            if (jobs == null)
            {
                return NotFound();
            }
            jobs.FullName = jobSeeker.FullName;
            jobs.Phone = jobSeeker.Phone;
            jobs.Address = jobSeeker.Address;
            jobs.Detail = jobSeeker.Detail;
            jobs.Email = jobSeeker.Email;
            jobs.CV = jobSeeker.CV;
            jobs.ApplicationUserId = jobSeeker.ApplicationUserId;

            _context.Update(jobs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        // GET: JobSeekers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.JobSeekers == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeekers
                .Include(j => j.ApplicationUser)
                .FirstOrDefaultAsync(m => m.JobSeekerId == id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        // POST: JobSeekers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.JobSeekers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobSeekers'  is null.");
            }

            var jobSeeker = await _context.JobSeekers
                .Include(j => j.ApplicationUser)
                .FirstOrDefaultAsync(m => m.JobSeekerId == id);

            if (jobSeeker != null)
            {
                if (jobSeeker.ApplicationUserId != null)
                {
                    var user = await _userManager.FindByIdAsync(jobSeeker.ApplicationUserId);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            return Problem("Failed to delete the associated user.");
                        }
                    }
                }

                _context.JobSeekers.Remove(jobSeeker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }



        private bool JobSeekerExists(string id)
        {
            return (_context.JobSeekers?.Any(e => e.JobSeekerId == id)).GetValueOrDefault();
        }
    }
}
