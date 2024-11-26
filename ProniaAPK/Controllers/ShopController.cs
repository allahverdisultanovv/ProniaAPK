﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;
using ProniaAPK.Models;
using ProniaAPK.ViewModels;

namespace ProniaAPK.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDBContext _context;

        public ShopController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id < 1 || id is null) return BadRequest();
            Product? product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.productImages.OrderByDescending(pi => pi.IsPrimary))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();
            DetailVM detailVM = new DetailVM
            {
                Product = product,
                RelatedProducts = await _context.Products
                .Take(8)
                .Where(p => p.Category.Id == product.CategoryId && p.Id != id)
                .Include(p => p.productImages.Where(pi => pi.IsPrimary != null))
                .ToListAsync()

            };
            return View(detailVM);
        }
    }
}
