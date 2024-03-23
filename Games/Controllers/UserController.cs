using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    public class UserController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private GameContext _db;

        public UserController(UserManager<IdentityUser> manager, GameContext context)
        {
            _userManager = manager;
            _db = context;
        }

        public async Task<IActionResult> Index()
        {
            var name = HttpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(name);

            var account = _db.Accounts.Find(user.Id);

            if (account == null)
            {
                account = new Account()
                {
                    Id = user.Id,
                    ReviewsNumber = 0
                };
            }

            return View(account);
        }
    }
}
