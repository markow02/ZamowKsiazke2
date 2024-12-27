using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Data;

namespace ZamowKsiazke.Controllers
{
    public class StoreController : Controller
    {

        private readonly ZamowKsiazkeContext _context;
        public StoreController(ZamowKsiazkeContext context) // Add constructor to initialize _context
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
                if (book == null)
                {
                    return NotFound();
                }

                return View(book);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


    }
}
