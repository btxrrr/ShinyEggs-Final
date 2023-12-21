using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ShinyEggs.Views.Products
{
	[Authorize(Roles = "Root Admin, Admin, Inventory Manager")]
	public class ProductsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Products
		[Authorize(Roles = "Root Admin, Admin, Inventory Manager")] // Add this authorization attribute
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Products.Include(p => p.Categories);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Categories)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}

			// Create an audit record for product details view
			var auditRecord = new AuditRecord
			{
				AuditActionType = "Product Details View",
				DateTimeStamp = DateTime.Now,
				Username = User.Identity.Name,
				KeyProductFieldID = product.Id,
				OriginalValues = "", // No original values for product details view
				NewValues = "User Viewed Product Details"
			};

			_context.AuditRecords.Add(auditRecord);
			await _context.SaveChangesAsync();

			return View(product);
		}

		// GET: Products/Create
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,ProductName,Brand,Price,CategoryId,BriefDescription,Image")] Product product)
		{
			var category = await _context.Category.FindAsync(product.CategoryId);
			ModelState.Clear();
			if (category == null)
			{
				ModelState.AddModelError("", "Please verify that all fields are valid.");
			}
			else if (ModelState.IsValid)
			{
				_context.Add(product);

				// Before saving changes, create an audit record
				if (await _context.SaveChangesAsync() > 0)
				{
					// Create an auditrecord object
					var auditRecord = new AuditRecord
					{
						AuditActionType = "Add Product Record",
						DateTimeStamp = DateTime.Now,
						KeyProductFieldID = product.Id,
						Username = User.Identity.Name,
						OriginalValues = JsonConvert.SerializeObject(new { }), // Empty JSON object
						NewValues = "New Product Created" // Set appropriate message here
					};

					_context.AuditRecords.Add(auditRecord); // Add the audit record
					await _context.SaveChangesAsync(); // Save changes to the database
				}

				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
			return View(product);
		}



		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Brand,Price,CategoryId,BriefDescription,Image")] Product product)
		{
			if (id != product.Id)
			{
				return NotFound();
			}

			var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

			if (originalProduct == null)
			{
				return NotFound();
			}

			var category = await _context.Category.FindAsync(product.CategoryId);
			ModelState.Clear();
			if (category == null)
			{
				ModelState.AddModelError("", "Please verify that all fields are valid.");
			}
			else if (ModelState.IsValid)
			{
				try
				{
					_context.Update(product);

					// Create an audit record for the edit
					var auditRecord = new AuditRecord
					{
						AuditActionType = "Edit Product Record",
						DateTimeStamp = DateTime.Now,
						KeyProductFieldID = product.Id,
						Username = User.Identity.Name,
						OriginalValues = JsonConvert.SerializeObject(originalProduct),
						NewValues = JsonConvert.SerializeObject(product)
					};

					_context.AuditRecords.Add(auditRecord);

					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(product.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", product.CategoryId);
			return View(product);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Categories)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Products == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
			}
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id); // Fetch original entity
				_context.Products.Remove(product);

				// Before saving changes, create an audit record
				if (await _context.SaveChangesAsync() > 0)
				{
					// Create an auditrecord object
					var auditRecord = new AuditRecord
					{
						AuditActionType = "Delete Product Record",
						DateTimeStamp = DateTime.Now,
						KeyProductFieldID = product.Id,
						Username = User.Identity.Name,
						OriginalValues = JsonConvert.SerializeObject(originalProduct), // Set original entity's JSON representation
						NewValues = "Product Deleted"
					};

					_context.AuditRecords.Add(auditRecord); // Add the audit record
					await _context.SaveChangesAsync(); // Save changes to the database
				}
			}

			return RedirectToAction(nameof(Index));
		}


		private bool ProductExists(int id)
		{
			return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
