using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppCMS.Data.DTOs;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private UserManager<AppUser> _userManager;
        private IPasswordHasher<AppUser> _passwordHasher;

        public UserController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }
        
        public IActionResult Index()
        {
            var users = _userManager.Users;

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _userManager.Users.FirstOrDefault(u => u.Id == GetCurrentUserId())
                };

                if (user.Avatar != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        user.Avatar.CopyTo(stream);
                        var file = stream.ToArray();

                        appUser.Avatar = file;
                    }
                }

                var result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            
            return View(user);
        }

        public async Task<IActionResult> EditDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var appUser = new EditAppUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                if (user.Avatar != null)
                {
                    appUser.AvatarAsBase64String = user.GetAvatarAsBase64String();
                }

                return View(appUser);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditDetails(string id, EditAppUserViewModel appUserViewModel)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.Id != id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingUser.UserName = appUserViewModel.UserName;
                existingUser.Email = appUserViewModel.Email;
                existingUser.ModifiedAt = DateTime.Now;
                existingUser.ModifiedBy = _userManager.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());

                if (appUserViewModel.Avatar != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        appUserViewModel.Avatar.CopyTo(stream);
                        var file = stream.ToArray();

                        existingUser.Avatar = file;
                    }
                }

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(appUserViewModel);
        }

        public async Task<IActionResult> ChangePassword(string id) 
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, string password)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (existingUser.Id != id)
            {
                return NotFound();
            }
                
            if (!string.IsNullOrEmpty(password))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, password);
                existingUser.ModifiedAt = DateTime.Now;
                existingUser.ModifiedBy = _userManager.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Password cannot be empty!");
            }
            
            return View(existingUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                string adminRoleName = "Admin";
                if (await _userManager.IsInRoleAsync(user, adminRoleName))
                {
                    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                    if (adminUsers.Count == 1)
                    {
                        ViewBag.UnableToDeleteMsg = "Unable to delete the only existing Admin user.";
                    }
                }
                
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                var result = await _userManager.DeleteAsync(appUser);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return RedirectToAction(nameof(Index));

        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }
    }
}
