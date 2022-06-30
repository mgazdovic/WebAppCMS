using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppCMS.Data.DTOs;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await GetAllUsersWithRoles();

            return View(usersWithRoles);
        }

        public async Task<IActionResult> ConfigureRolesForUser(string id)
        {
            ViewBag.UserId = id;

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserName = user.UserName;
            var model = new List<ConfigureUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var configureUserRole = new ConfigureUserRolesViewModel()
                {
                    Id = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    configureUserRole.Selected = true;
                }
                else
                {
                    configureUserRole.Selected = false;
                }

                model.Add(configureUserRole);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfigureRolesForUser(List<ConfigureUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            string adminRoleName = "Admin";
            if (await _userManager.IsInRoleAsync(user, adminRoleName))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                if (adminUsers.Count == 1)
                {
                    TempData["UnableToRemoveMsg"] = "Unable to remove Admin role from the only existing Admin user.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove roles from existing user");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync
                (user, model.Where(role => role.Selected == true).Select(role => role.RoleName));

            if (result.Succeeded)
            {
                user.ModifiedAt = DateTime.Now;
                user.ModifiedBy = _userManager.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());
                var userResult = await _userManager.UpdateAsync(user);

                if (!userResult.Succeeded)
                {
                    ModelState.AddModelError("", "User update failed!");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }

        private async Task<List<UserRolesViewModel>> GetAllUsersWithRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var userWithRole = new UserRolesViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = new List<string>(await _userManager.GetRolesAsync(user))
                };

                usersWithRoles.Add(userWithRole);
            }

            return usersWithRoles;
        }
    }
}
