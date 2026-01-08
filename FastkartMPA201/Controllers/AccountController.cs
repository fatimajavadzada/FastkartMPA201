using System.Threading.Tasks;
using FastkartMPA201.Models;
using FastkartMPA201.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FastkartMPA201.Abstraction;

namespace FastkartMPA201.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IEmailService _service) : Controller
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

            var existUser = await _userManager.FindByNameAsync(vm.UserName);

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

            await _signInManager.SignInAsync(appUser, false);
            TempData["SuccessMessage"] = "Registerden ugurlu kecdiniz,indi zehmet olmasa email-ni tesdiqleyin";
            await SendConfirmationEmail(appUser);

            return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        private async Task SendConfirmationEmail(AppUser user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var url = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

            string emailBody = @$"<!doctype html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
  <meta name=""x-apple-disable-message-reformatting"" />
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" />
  <title>Confirmation</title>
  <!--[if mso]>
  <noscript>
    <xml>
      <o:OfficeDocumentSettings>
        <o:PixelsPerInch>96</o:PixelsPerInch>
      </o:OfficeDocumentSettings>
    </xml>
  </noscript>
  <![endif]-->
  <style>
    /* Basic resets */
    html, body {{ margin:0 !important; padding:0 !important; width:100% !important; height:100% !important; }}
    * {{ -ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; }}
    table, td {{ mso-table-lspace:0pt; mso-table-rspace:0pt; border-collapse:collapse !important; }}
    img {{ -ms-interpolation-mode:bicubic; border:0; outline:none; text-decoration:none; display:block; }}
    a {{ text-decoration:none; }}
    /* Mobile */
    @media screen and (max-width:600px) {{
      .container {{ width:100% !important; }}
      .px {{ padding-left:16px !important; padding-right:16px !important; }}
      .btn {{ width:100% !important; }}
    }}
  </style>
</head>

<body style=""background:#f5f7fb; font-family: Arial, Helvetica, sans-serif;"">
  <!-- Preheader (hidden) -->
  <div style=""display:none; font-size:1px; line-height:1px; max-height:0px; max-width:0px; opacity:0; overflow:hidden;"">
    {{{{preheader_text}}}}
  </div>

  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background:#f5f7fb;"">
    <tr>
      <td align=""center"" style=""padding:24px 12px;"">
        <!-- Container -->
        <table role=""presentation"" class=""container"" width=""600"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:600px; max-width:600px;"">
          <!-- Header -->
          <tr>
            <td align=""left"" class=""px"" style=""padding:16px 24px;"">
              <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                <tr>
                  <td align=""left"" style=""font-size:18px; font-weight:700; color:#111827;"">
                    Fastkart
                  </td>
                  
                </tr>
              </table>
            </td>
          </tr>

          <!-- Card -->
          <tr>
            <td style=""background:#ffffff; border-radius:12px; overflow:hidden;"">
              <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                <!-- Hero -->
                <tr>
                  <td class=""px"" style=""padding:28px 24px 8px;"">
                    <h1 style=""margin:0; font-size:22px; line-height:30px; color:#111827;"">
                      Confirm Email ✅
                    </h1>
                    <p style=""margin:12px 0 0; font-size:14px; line-height:22px; color:#374151;"">
                      Hi {user.FullName},<br/>
                      Thanks! Your request has been confirmed.
                    </p>
                  </td>
                </tr>

                <!-- Details box -->
                <tr>
                  <td class=""px"" style=""padding:16px 24px;"">
                    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background:#f9fafb; border:1px solid #e5e7eb; border-radius:10px;"">
                      <tr>
                        <td style=""padding:14px 14px;"">
                          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                            
                         
                            <tr>
                              <td style=""font-size:12px; color:#6b7280; padding:2px 0;"">Status</td>
                              <td align=""right"" style=""font-size:12px; color:#065f46; padding:2px 0; font-weight:700;"">Confirmed</td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- CTA -->
                <tr>
                  <td class=""px"" style=""padding:10px 24px 24px;"">
                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" class=""btn"">
                      <tr>
                        <td align=""center"" style=""background:#2563eb; border-radius:10px;"">
                          <a href=""{url}"" target=""_blank""
                             style=""display:inline-block; padding:12px 18px; font-size:14px; line-height:16px; color:#ffffff; font-weight:700;"">
                            Link-e click edin.
                          </a>
                        </td>
                      </tr>
                    </table>

                    <p style=""margin:14px 0 0; font-size:12px; line-height:18px; color:#6b7280;"">
                      If the button doesn’t work, copy and paste this link into your browser:<br/>
                      <a href=""{url}"" target=""_blank"" style=""color:#2563eb; word-break:break-all;"">{url}</a>
                    </p>
                  </td>
                </tr>

                <!-- Support -->
                <tr>
                  <td class=""px"" style=""padding:0 24px 24px;"">
                    <p style=""margin:0; font-size:12px; line-height:18px; color:#6b7280;"">
                      Need help? Contact us at
                      <a href=""mailto:{user.Email}"" style=""color:#2563eb;"">{user.Email}</a>.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td align=""center"" class=""px"" style=""padding:18px 24px; font-size:11px; line-height:16px; color:#6b7280;"">
              © 2026 Fastkart. All rights reserved.<br/>
              Fastkart Team<br/>
              
            </td>
          </tr>
        </table>
        <!-- /Container -->
      </td>
    </tr>
  </table>
</body>
</html>
";

            await _service.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is not { })
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return BadRequest();
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
    }
}
