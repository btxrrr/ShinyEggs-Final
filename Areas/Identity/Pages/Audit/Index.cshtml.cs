using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShinyEggs.Data;
using ShinyEggs.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShinyEggs.Areas.Identity.Pages.Audit
{
	[Authorize(Roles = "Root Admin, Admin, Audit Manager")]
	public class IndexModel : PageModel
	{
		private readonly ShinyEggs.Data.ApplicationDbContext _context;

		public IndexModel(ShinyEggs.Data.ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<AuditRecord> AuditRecord { get; set; }

		public async Task OnGetAsync()
		{
			if (_context.AuditRecords != null)
			{
				AuditRecord = await _context.AuditRecords.ToListAsync();
			}
		}
	}
}
