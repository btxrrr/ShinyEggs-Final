using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShinyEggs.Data;
using ShinyEggs.Models;

namespace ShinyEggs.Areas.Identity.Pages.Audit
{
    public class CreateModel : PageModel
    {
        private readonly ShinyEggs.Data.ApplicationDbContext _context;

        public CreateModel(ShinyEggs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

		public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AuditRecord AuditRecord { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.AuditRecords == null || AuditRecord == null)
            {
                return Page();
            }

            _context.AuditRecords.Add(AuditRecord);

			// Create an audit record for the creation of an audit record
			var auditRecord = new AuditRecord
			{
				AuditActionType = "Create Audit Record",
				DateTimeStamp = DateTime.Now,
				Username = User.Identity.Name,
				KeyProductFieldID = 0, // No specific product ID for audit record creation
				OriginalValues = "", // No original values for audit record creation
				NewValues = "Audit Record Created"
			};

			_context.AuditRecords.Add(auditRecord);

			await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
