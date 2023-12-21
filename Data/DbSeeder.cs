using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShinyEggs.Constants;
using ShinyEggs.Models;
using System;
using System.Diagnostics.Metrics;

namespace ShinyEggs.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
			// Get the database context service.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
			using (var context = new ApplicationDbContext(service.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
			{
                // Check if there is any data in the database
                if (context.OrderStats.Any())
                {
                    return; // DB has been seeded
                }

				context.OrderStats.AddRange(
				   new OrderStatus { StatusName = "Pending", StatusId = 1 },
				   new OrderStatus { StatusName = "Shipped", StatusId = 2 },
				   new OrderStatus { StatusName = "Delivered", StatusId = 3 },
				   new OrderStatus { StatusName = "Cancelled", StatusId = 4 },
				   new OrderStatus { StatusName = "Returned", StatusId = 5 },
				   new OrderStatus { StatusName = "Refunded", StatusId = 6 }
				   );

				context.SaveChanges();

				// Check if there is any data in the database
				if (context.Category.Any())
				{
					return; // DB has been seeded
				}

				context.Category.AddRange(
                    new Categories {CategoryName = "Laptops" },
                    new Categories {CategoryName = "Mobiles and Tablets" },
                    new Categories {CategoryName = "Audio" },
                    new Categories {CategoryName = "Accessories" }
                    );

                context.SaveChanges();

                context.Products.AddRange(
                   new Product
                   {
                       ProductName = "13-INCH MACBOOK AIR (M1 CHIP)",
                       Price = 1499,
                       CategoryId = 1,
                       BriefDescription = "8-Core CPU",
                       Image = "macbookair.jpeg",
                       Brand = "Apple"
                   },

                   new Product
                   {
                       ProductName = "XPS 13 Plus Laptop",
                       Price = 2199,
                       CategoryId = 1,
                       BriefDescription = "Processor\r\n13th Generation Intel® Core™ i5-1340P (12MB Cache, up to 4.6 GHz, 12 cores)",
                       Image = "XPS.jpg",
                       Brand = "Dell"
                   },

                   new Product
                   {
                       ProductName = "Galaxy A14 5G",
                       Price = 318,
                       CategoryId = 2,
                       BriefDescription = "6.6\" Display\r\n50MP Camera\r\nOcta core processor",
                       Image = "samsung.jpg",
                       Brand = "Samsung"
                   },

                   new Product
                   {
                       ProductName = "iPad mini",
                       Price = 756,
                       CategoryId = 2,
                       BriefDescription = "Wi-Fi\r\nEvery iPad can connect to Wi‑Fi, so you can stay connected. ",
                       Image = "ipadmini.jpg",
                       Brand = "Apple"
                   },

                   new Product
                   {
                       ProductName = "WH-1000XM4 Wireless Noise Cancelling Headphones",
                       Price = 504,
                       CategoryId = 3,
                       BriefDescription = "HD Noise Cancelling Processor QN1, Bluetooth Audio SoC and a dual noise sensor let you listen without distractions\r\nPersonal Noise Cancelling Optimiser and Atmospheric Pressure Optimising\r\nHigh-quality audio with DSEE Extreme™ and LDAC\r\nAdaptive Sound Control automatically adjusts ambient sound settings to suit your location and behaviour\r\nWireless freedom with BLUETOOTH® technology",
                       Image = "xm4.jpg",
                       Brand = "Sony"
                   },
                   new Product
                   {
                       ProductName = "Marshall Stanmore II Bluetooth Speaker, Brown",
                       Price = 376.37,
                       CategoryId = 3,
                       BriefDescription = "\tNoise Cancelling, Touch Panel, Ultra Portable Bluetooth Speaker, Lasts 8-10 hours with single charge",
                       Image = "marshall.jpg",
                       Brand = "Marshall"
                   },
                   new Product
                   {
                       ProductName = "USB-C to Lightning Cable (2 m)",
                       Price = 45.4,
                       CategoryId = 4,
                       BriefDescription = "Apple USB-C to Lightning Cable (2 m)",
                       Image = "cable.jpg",
                       Brand = "Apple"
                   },
                   new Product
                   {
                       ProductName = "Apple Pencil (1st Generation)",
                       Price = 150.35,
                       CategoryId = 4,
                       BriefDescription = "Apple Pencil (1st Generation)",
                       Image = "applepencil.jpg",
                       Brand = "Apple"
                   },
                   new Product
                   {
                       ProductName = "Test",
                       Price = 100,
                       CategoryId = 4,
                       BriefDescription = "test",
                       Image = null,
                       Brand = "Apple"
                   },
				   new Product
				   {
					   ProductName = "Test333",
					   Price = 100,
					   CategoryId = 4,
					   BriefDescription = "test",
					   Image = null,
					   Brand = "Apple"
				   }
				   );
                context.SaveChanges();
            }



            // Get the services we need to add users and roles
            var userMgr = service.GetService<UserManager<ApplicationUser>>();
            var roleMgr = service.GetService<RoleManager<ApplicationRole>>();

			// Check if there are any existing users and roles in the database
			if (userMgr.Users.Any() || roleMgr.Roles.Any())
			{
				return; // DB has been seeded
			}

			//adding some roles to db
			await roleMgr.CreateAsync(new ApplicationRole { Name = "Inventory Manager" }); 
			await roleMgr.CreateAsync(new ApplicationRole { Name = "Audit Manager" });
			await roleMgr.CreateAsync(new ApplicationRole { Name = "Roles Manager" });
			await roleMgr.CreateAsync(new ApplicationRole { Name = "Root Admin" });
			await roleMgr.CreateAsync(new ApplicationRole { Name = "Admin" }); //for testing purposes, admin same functionality as Root Admin, but no security
			await roleMgr.CreateAsync(new ApplicationRole { Name = "User" });

			//create admin user (tester)
			var admin = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var adminInDb = await userMgr.FindByEmailAsync(admin.Email);
            if(adminInDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123!");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }

			//create Inventory Manager Admin
			var inventoryMgr = new ApplicationUser
			{
				UserName = "inventoryMgr@gmail.com",
				Email = "inventoryMgr@gmail.com",
				EmailConfirmed = true
			};

			var inventoryMgrInDb = await userMgr.FindByEmailAsync(inventoryMgr.Email);
			if (inventoryMgrInDb is null)
			{
				await userMgr.CreateAsync(inventoryMgr, "IvMGR@123!");
				await userMgr.AddToRoleAsync(inventoryMgr, "Inventory Manager");
			}

			//create Audit Manager Admin
			var auditMgr = new ApplicationUser
			{
				UserName = "auditMgr@gmail.com",
				Email = "auditMgr@gmail.com",
				EmailConfirmed = true
			};

			var auditMgrInDb = await userMgr.FindByEmailAsync(auditMgr.Email);
			if (auditMgrInDb is null)
			{
				await userMgr.CreateAsync(auditMgr, "AuMGR@123!");
				await userMgr.AddToRoleAsync(auditMgr, "Audit Manager");
			}

			//create Roles Manager Admin
			var rolesMgr = new ApplicationUser  //need to edit in database from role to roles
			{
				UserName = "rolesMgr@gmail.com",
				Email = "rolesMgr@gmail.com",
				EmailConfirmed = true
			};

			var rolesMgrInDb = await userMgr.FindByEmailAsync(rolesMgr.Email);
			if (rolesMgrInDb is null)
			{
				await userMgr.CreateAsync(rolesMgr, "RoMGR@123!");
				await userMgr.AddToRoleAsync(rolesMgr, "Roles Manager");
			}

			//create Root Admin
			var rootAdmin = new ApplicationUser  
			{
				UserName = "rootAdmin@gmail.com",
				Email = "rootAdmin@gmail.com",
				EmailConfirmed = true,
				//TwoFactorEnabled = true
			};

			var rootAdminInDb = await userMgr.FindByEmailAsync(rootAdmin.Email);
			if (rootAdminInDb is null)
			{
				await userMgr.CreateAsync(rootAdmin, "R00t@123!");
				await userMgr.AddToRoleAsync(rootAdmin, "Root Admin");
			}

			//create User
			var user = new ApplicationUser
			{
				UserName = "user@gmail.com",
				Email = "user@gmail.com",
				EmailConfirmed = true
			};

			var userInDb = await userMgr.FindByEmailAsync(user.Email);
			if (userInDb is null)
			{
				await userMgr.CreateAsync(user, "User@123!");
				await userMgr.AddToRoleAsync(user, "User");
			}
		}
    }
}
