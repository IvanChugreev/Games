using Games.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Games.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [ActionName("GetUsers")]
        public async Task<IActionResult> GetUsersAsync()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        [ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("GetUsers");
        }

        [ActionName("EditRole")]
        public async Task<IActionResult> EditRoleAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                AllRoles = await _roleManager.Roles.ToListAsync(),
                UserRoles = await _userManager.GetRolesAsync(user)
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("EditRole")]
        public async Task<IActionResult> EditRoleAsync(string id, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles.Except(roles));

            await _userManager.AddToRolesAsync(user, roles.Except(userRoles));

            return RedirectToAction("GetUsers");
        }

        [ActionName("GetRoles")]
        public async Task<IActionResult> GetRolesAsync()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        [ActionName("AddRole")]
        public async Task<IActionResult> AddRoleAsync()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        [HttpPost]
        [ActionName("AddRole")]
        public async Task<IActionResult> AddRoleAsync(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [ActionName("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);

            if (role != null)
            {
                return View(role);
            }

            return NotFound();
        }

        [HttpPost]
        [ActionName("DeleteRole")]
        public async Task<IActionResult> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("GetRoles");
        }
    }
}
