using System.Threading.Tasks;
using FastkartMPA201.Models;
using FastkartMPA201.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastkartMPA201.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByEmailAsync(vm.UserName);

            if (existUser is { })
            {
                ModelState.AddModelError("UserName", "This username is already exist~");
                return View(vm);
            }

            existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);

            if (existUser is { })
            {
                ModelState.AddModelError("EmailAddress", "This email is already exist!");
                return View(vm);
            }

            AppUser appUser = new AppUser()
            {
                FullName = vm.FullName,
                UserName = vm.UserName,
                Email = vm.EmailAddress
            };

            var result = await _userManager.CreateAsync(appUser, vm.Password);

            if (result.Succeeded == false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }

            return Ok("User Registered Successfully!");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);

            if (user is not { })
            {
                ModelState.AddModelError(" ", "Email or password is wrong");
                return View(vm);
            }

            var result = await _userManager.CheckPasswordAsync(user, vm.Password);

            if (result is false)
            {
                ModelState.AddModelError(" ", "Email or password is wrong");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.isRemember);

            return Ok("User Loged in Successfully!");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

    }
}
