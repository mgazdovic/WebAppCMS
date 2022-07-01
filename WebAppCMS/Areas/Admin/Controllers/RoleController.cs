using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

                if (!result.Succeeded)
                {
                    string errors = "";
                    foreach (var error in result.Errors)
                    {
                        errors += " | " + error.Description;
                    }
                    TempData["ErrorMessage"] = errors;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (roleName != null)
            {
                if (roleName == "Admin" || roleName == "Supervisor")
                {
                    TempData["ErrorMessage"] = "Unable to delete system role.";
                }
                else
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    var result = await _roleManager.DeleteAsync(role);

                    if (!result.Succeeded)
                    {
                        string errors = "";
                        foreach (var error in result.Errors)
                        {
                            errors += " | " + error.Description;
                        }
                        TempData["ErrorMessage"] = errors;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
