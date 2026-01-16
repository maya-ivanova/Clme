using Clme.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clme.Controllers
    {
    public class ProductsController : Controller
        {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
            {
            _context = context;
            }

        // GET: /Products
        public async Task<IActionResult> Index()
            {
            var products = await _context.Products.ToListAsync();
            return View(products);
            }
        }
    }
