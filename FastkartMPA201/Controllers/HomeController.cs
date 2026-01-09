using System.Security.Claims;
using FastkartMPA201.Contexts;
using FastkartMPA201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastkartMPA201.Controllers
{
    public class HomeController(AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
        public async Task<IActionResult> AddToBasket(int productId)
        {
            var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
            if (isExistProduct == false)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

            if (!isExistUser)
            {
                return BadRequest();
            }
            var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
            if (existBasketItem is { })
            {
                existBasketItem.Count++;
                _context.BasketItems.Update(existBasketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                BasketItem basketItem = new()
                {
                    ProductId = productId,
                    AppUserId = userId,
                    Count = 1
                };
                await _context.BasketItems.AddAsync(basketItem);

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
