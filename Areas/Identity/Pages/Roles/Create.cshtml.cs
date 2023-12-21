using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShinyEggs.Models;
using System;
using System.Threading.Tasks;

namespace ShinyEggs.Pages.Roles
{
	[Authorize(Roles = "Root Admin, Admin, Roles Manager")]
	public class CreateModel : PageModel
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public CreateModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
		{
			_roleManager = roleManager;
			_context = context;
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public ApplicationRole ApplicationRole { get; set; }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			ApplicationRole.CreatedDate = DateTime.UtcNow;
			ApplicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

			IdentityResult roleResult = await _roleManager.CreateAsync(ApplicationRole);

			if (roleResult.Succeeded)
			{
				// Log audit record for role creation
				var auditRecord = new AuditRecord
				{
					AuditActionType = "Role Creation",
					Username = User.Identity.Name,
					DateTimeStamp = DateTime.Now,
					KeyProductFieldID = 0, // Use an appropriate identifier
					OriginalValues = "",
					NewValues = "Role Created"
				};

				_context.AuditRecords.Add(auditRecord);
				await _context.SaveChangesAsync();

				return RedirectToPage("Index");
			}
			else
			{
				// Handle role creation failure (return to page with error messages)
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return Page();
			}
		}
	}
}
