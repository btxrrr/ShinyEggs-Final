using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShinyEggs.Models;

namespace ShinyEggs.Pages.Roles
{
	[Authorize(Roles = "Root Admin, Admin, Roles Manager")]
	public class DeleteModel : PageModel
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public DeleteModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
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

			IdentityResult roleResult = await _roleManager.DeleteAsync(appRole);

			if (roleResult.Succeeded)
			{
				// Log audit record for role deletion
				var auditRecord = new AuditRecord
				{
					AuditActionType = "Role Deletion",
					Username = User.Identity.Name,
					DateTimeStamp = DateTime.Now,
					KeyProductFieldID = 0, // Use role name as the identifier
					OriginalValues = "Name: " + appRole.Name + "; Description: " + appRole.Description,
					NewValues = "Role Deleted"
				};

				_context.AuditRecords.Add(auditRecord);
				await _context.SaveChangesAsync();

				return RedirectToPage("Index");
			}
			else
			{
				// Handle role deletion failure (return to page with error messages)
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return Page();
			}
		}
	}
}
