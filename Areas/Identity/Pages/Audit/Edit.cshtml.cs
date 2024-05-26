using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShinyEggs.Data;
using ShinyEggs.Models;

namespace ShinyEggs.Areas.Identity.Pages.Audit
{
    public class EditModel : PageModel
    {
        private readonly ShinyEggs.Data.ApplicationDbContext _context;

        public EditModel(ShinyEggs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditRecord AuditRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AuditRecords == null)
            {
                return NotFound();
            }

            var auditrecord =  await _context.AuditRecords.FirstOrDefaultAsync(m => m.Audit_ID == id);
            if (auditrecord == null)
            {
                return NotFound();
            }
            AuditRecord = auditrecord;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AuditRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditRecordExists(AuditRecord.Audit_ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AuditRecordExists(int id)
        {
          return (_context.AuditRecords?.Any(e => e.Audit_ID == id)).GetValueOrDefault();
        }
    }
}
