using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ShinyEggs.Models;

namespace ShinyEggs.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly ShinyEggs.Data.ApplicationDbContext _context;

		public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, ShinyEggs.Data.ApplicationDbContext context)
		{
			_signInManager = signInManager;
			_logger = logger;
			_context = context;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public string ReturnUrl { get; set; }

		[TempData]
		public string ErrorMessage { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");

			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;

			Input?.Email?.ToLower();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			try
			{
				returnUrl ??= Url.Content("~/");

				ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

				if (ModelState.IsValid)
				{
					Input.Email = Input.Email?.ToLower();
					var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
					if (result.Succeeded)
					{
						_logger.LogInformation("User logged in.");
						return LocalRedirect(returnUrl);
					}
					if (result.RequiresTwoFactor)
					{
						return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
					}
					if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");

                        // Log the failed lockout attempt
                        var lockoutAuditRecord = new AuditRecord
                        {
                            AuditActionType = "Account Lockout",
                            DateTimeStamp = DateTime.Now,
                            Username = Input.Email,
                            KeyProductFieldID = 0, // No specific ID for lockout
                            OriginalValues = "", // No original values for lockout
                            NewValues = "User Locked Out"
                        };

                        _context.AuditRecords.Add(lockoutAuditRecord);
                        await _context.SaveChangesAsync();

                        return RedirectToPage("./Lockout");
                    }
                    // Handle other cases
                    else
                    {
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                            // Log the failed login attempt
                            var failedLoginAuditRecord = new AuditRecord
                            {
                                AuditActionType = "Failed Login Attempt",
                                DateTimeStamp = DateTime.Now,
                                Username = Input.Email,
                                KeyProductFieldID = 0, // No specific ID for failed login
                                OriginalValues = "", // No original values for failed login
                                NewValues = "User Failed to Log In"
                            };

                            _context.AuditRecords.Add(failedLoginAuditRecord);
                            await _context.SaveChangesAsync();

                            _logger.LogWarning("Invalid login attempt for email: {Email}", Input.Email);

                            return Page();
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing the login attempt.");
				ModelState.AddModelError(string.Empty, "An error occurred while processing the login attempt.");
				return Page();
			}
		}
	}
}
