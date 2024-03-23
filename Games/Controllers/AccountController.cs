using Games.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser() { Email = model.Email, UserName = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var callbackUrl = Url.Action(
                        "ConfirmEmail", 
                        "Account", 
                        new { userId = user.Id, code = await _userManager.GenerateEmailConfirmationTokenAsync(user) }, 
                        protocol: HttpContext.Request.Scheme);

                    await EmailService.SendEmailAsync(
                        model.Email, 
                        "Confirm your account", 
                        $"Confirm registration by clicking on the link: <a href='{callbackUrl}'>link</a>");

                    return Content("To complete the registration, check your email and follow the link provided in the email.");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [ActionName("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");

                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Incorrect login and/or password.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email address not confirmed.");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel { UserId = user.Id, UserEmail = user.Email };

            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return View("Success");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return View("ForgotPasswordConfirmation");
            }

            var callBackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { userId = user.Id, code = await _userManager.GeneratePasswordResetTokenAsync(user) },
                HttpContext.Request.Scheme);

            await EmailService.SendEmailAsync(
                model.Email,
                "Confirm your account",
                $"To reset your password, follow the link: <a href='{callBackUrl}'>link</a>");

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }

            foreach (var e in result.Errors)
            {
                ModelState.AddModelError(string.Empty, e.Description);
            }

            return View(model);
        }
    }
}
