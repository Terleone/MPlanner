using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPlanner.Data;
using MPlanner.Models;

namespace MPlanner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IEmailSender _emailSender;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUserAsync().Result;
            return View(await _context.Users.Where(x => x.Id != user.Id).ToListAsync());
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            _context.Movie.RemoveRange(_context.Movie.Where(x => x.UserId == user.Id));
            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return View("UserRemovalFailed");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResendConfirmationMail(string id)
        {
            var user = await _context.Users.FindAsync(id);
            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Movies/Index",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Rights()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantRights([Bind("Email")] RightsGrantingModel viewModel)
        {
            const string notGranted = "Admin rights were not granted";
            const string granted = "Admin rights were granted";
            const string alreadyAnAdmin = "User is already an admin";
            const string doesntExist = "User with given e-mail address does not exist.";
            const string cantYourself = "You're already an admin.";

            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(viewModel.Email).Result;
                var currentUser = GetCurrentUserAsync();

                if (user == null)
                {
                    ViewData["Effect"] = notGranted;
                    ViewData["Reason"] = doesntExist;
                }
                else if (user.Id == currentUser.Result.Id)
                {
                    ViewData["Effect"] = notGranted;
                    ViewData["Reason"] = cantYourself;
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        ViewData["Effect"] = notGranted;
                        ViewData["Reason"] = alreadyAnAdmin;
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                        await _context.SaveChangesAsync();

                        ViewData["Effect"] = granted;
                    }
                }

                return View("RightsChangingEffect");
            }

            return View("Rights");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetractRights([Bind("Email")] RightsGrantingModel viewModel)
        {
            const string notRetracted = "Admin rights were not retracted";
            const string retracted = "Admin right were retracted";
            const string notAnAdmin = "User is not an admin.";
            const string doesntExist = "User with given e-mail address does not exist.";
            const string cantYourself = "You cannot retract your own rights.";

            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(viewModel.Email).Result;
                var currentUser = GetCurrentUserAsync();

                if (user == null)
                {
                    ViewData["Effect"] = notRetracted;
                    ViewData["Reason"] = doesntExist;
                }
                else if (user.Id == currentUser.Result.Id)
                {
                    ViewData["Effect"] = notRetracted;
                    ViewData["Reason"] = cantYourself;
                }
                else if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                        await _context.SaveChangesAsync();

                        ViewData["Effect"] = retracted;
                    }
                    else
                    {
                        ViewData["Effect"] = notRetracted;
                        ViewData["Reason"] = notAnAdmin;
                    }
                }

                return View("RightsChangingEffect");
            }

            return View("Rights");
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(x => x.Id == id);
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}