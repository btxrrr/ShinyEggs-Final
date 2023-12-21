using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShinyEggs.Models;

namespace ShinyEggs.Pages.Roles
{
	[Authorize(Roles = "Root Admin, Admin, Roles Manager")]
	public class EditModel : PageModel
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public EditModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
		{
			_roleManager = roleManager;
			_context = context;
		}

		[BindProperty]
		public ApplicationRole ApplicationRole { get; set; }

		public async Task<IActionResult> OnGetAsync(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			ApplicationRole = await _roleManager.FindByIdAsync(id);

			if (ApplicationRole == null)
			{
				return NotFound();
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			ApplicationRole appRole = await _roleManager.FindByIdAsync(ApplicationRole.Id);
			string originalName = appRole.Name; // Store the original role name before update
			appRole.Name = ApplicationRole.Name;
			appRole.Description = ApplicationRole.Description;

			IdentityResult roleResult = await _roleManager.UpdateAsync(appRole);

			if (roleResult.Succeeded)
			{
				// Log audit record for role edit
				var auditRecord = new AuditRecord
				{
					AuditActionType = "Role Edit",
					Username = User.Identity.Name,
					DateTimeStamp = DateTime.Now,
					KeyProductFieldID = 0, // Use the original role name as KeyProductFieldID
					OriginalValues = "Name: " + originalName + "; Description: " + appRole.Description,
					NewValues = "Role Edited"
				};

				_context.AuditRecords.Add(auditRecord);
				await _context.SaveChangesAsync();

				return RedirectToPage("Index");
			}
			else
			{
				// Handle role edit failure (return to page with error messages)
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return Page();
			}
		}
	}
}
