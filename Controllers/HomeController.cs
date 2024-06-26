﻿using Microsoft.AspNetCore.Mvc;
using ShinyEggs.Models;
using ShinyEggs.Models.DTOs;
using ShinyEggs.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ShinyEggs.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IHomeRepository _homeRepository;

		public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
		{
			_homeRepository = homeRepository;
			_logger = logger;
		}

		public async Task<IActionResult> Index(string sterm = "", int categoryId = 0)
		{
			IEnumerable<Product> products = await _homeRepository.GetProducts(sterm, categoryId);

			IEnumerable<Categories> category = await _homeRepository.Category();
			ProductDisplayModel productModel = new ProductDisplayModel
			{
				Products = products,
				Category = category,
				STerm = sterm,
				CategoryId = categoryId
			};
			return View(productModel);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}