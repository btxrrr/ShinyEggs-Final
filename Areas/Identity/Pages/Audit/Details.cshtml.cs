using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShinyEggs.Data;
using ShinyEggs.Models;

namespace ShinyEggs.Areas.Identity.Pages.Audit
{
    public class DetailsModel : PageModel
    {
        private readonly ShinyEggs.Data.ApplicationDbContext _context;

        public DetailsModel(ShinyEggs.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public AuditRecord AuditRecord { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AuditRecords == null)
            {
                return NotFound();
            }

            var auditrecord = await _context.AuditRecords.FirstOrDefaultAsync(m => m.Audit_ID == id);
            if (auditrecord == null)
            {
                return NotFound();
            }
            else 
            {
                AuditRecord = auditrecord;
            }
            return Page();
        }

    }
}
