using FastkartMPA201.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastkartMPA201.Controllers
{
    public class BasketController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var basketItems = await _context.BasketItems.Include(x => x.Product).ToListAsync();
            return View(basketItems);
        }

        public async Task<IActionResult> EmptyBasket()
        {
            var basketItems = await _context.BasketItems.ToListAsync();
            foreach (var basketItem in basketItems)
            {
                _context.BasketItems.Remove(basketItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
