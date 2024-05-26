using Google.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShinyEggs;
using ShinyEggs.Data;
using ShinyEggs.Models;
using System.Web.Mvc;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	//options.Password.RequireDigit = false;
	//options.Password.RequireLowercase = false;
	//options.Password.RequireNonAlphanumeric = false;
	//options.Password.RequireUppercase = false;
	//options.Password.RequiredLength = 5;
	//options.Password.RequiredUniqueChars = 1;

	// Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
	options.Lockout.MaxFailedAccessAttempts = 10; //during testing set to 3
	options.Lockout.AllowedForNewUsers = true;
	// User settings.
	options.User.AllowedUserNameCharacters =
	"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultUI()
	.AddDefaultTokenProviders();

//Session Timeout
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true; // Make the session cookie essential for GDPR compliance
});


builder.Services.ConfigureApplicationCookie(options =>
{
	// Session Management
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
	options.SlidingExpiration = false;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;


	options.Events.OnSignedIn = async context =>
	{
		// Log user login
		var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
		var user = await userManager.GetUserAsync(context.HttpContext.User);
		if (user != null)
		{
			var auditRecord = new AuditRecord
			{
				AuditActionType = "User Login",
				DateTimeStamp = DateTime.Now,
				Username = user.UserName, // User's username
				KeyProductFieldID = 0, // No specific product ID for login
				OriginalValues = "", // No original values for login
				NewValues = "User Logged In"
			};

			var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
			dbContext.AuditRecords.Add(auditRecord);
			await dbContext.SaveChangesAsync();
		}
	};

	options.Events.OnSigningOut = async context =>
	{
		// Log user logout
		var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
		var user = await userManager.GetUserAsync(context.HttpContext.User);
		var auditRecord = new AuditRecord
		{
			AuditActionType = "User Logout",
			DateTimeStamp = DateTime.Now,
			Username = user.UserName, // User's username
			KeyProductFieldID = 0, // No specific product ID for logout
			OriginalValues = "", // No original values for logout
			NewValues = "User Logged Out"
		};

		var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
		dbContext.AuditRecords.Add(auditRecord);
		await dbContext.SaveChangesAsync();
	};
});

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();





var app = builder.Build();

//Seeding data
using (var scope = app.Services.CreateScope())
{
	await DbSeeder.SeedDefaultData(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();



//Configuring session middleware
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
			name: "captcha",
			pattern: "{controller}/{action}/{id}",
			defaults: new { controller = "Captcha", action = "Index", id = UrlParameter.Optional });


app.MapControllerRoute(
    name: "payment",
    pattern: "Payment/Checkout", // This should match your payment page's route
    defaults: new { controller = "Payment", action = "Checkout" }); // Update action to match the actual action name

app.MapRazorPages();

app.Run();
