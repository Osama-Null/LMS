using LMS.Data;
using LMS.Models;
using LMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class AccountController : Controller
    {
        #region InjectedServices
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> SignInManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = SignInManager;
            _roleManager = roleManager;
            _context = context;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult RegisterStep1()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegisterStep1(RegisterStep1ViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Email"] = model.Email;
                TempData["Password"] = model.Password;
                TempData["ConfirmPassword"] = model.ConfirmPassword;
                return RedirectToAction("RegisterStep2");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult RegisterStep2()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel model, IFormFile profilePicture)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["Email"].ToString();
                var password = TempData["Password"].ToString();

                string imagePath = null;
                if (model.ProfilePicture != null)
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var filePath = Path.Combine(uploadsDir, model.ProfilePicture.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }
                    imagePath = model.ProfilePicture.FileName;
                }

                AppUser user = new AppUser
                {
                    Email = email,
                    UserName = email,
                    Gender = (AppUser.Genders)model.Gender,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    CivilID = model.CivilID,
                    Balance = 0,
                    AccountNumber = AppUser.GenerateAccountNumber(),
                    Img = imagePath // Add profile picture path
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

    }
}
