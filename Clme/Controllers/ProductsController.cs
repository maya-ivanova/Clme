using Clme.Data;
using Clme.Models.ViewModels.Products;
using Clme.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clme.Controllers
    {
    public class ProductsController : Controller
        {

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
            {
            _productService = productService;
            }

        public async Task<IActionResult> Index()
            {
            var products = await _productService.GetAllAsync();

            var viewModel = new ProductListViewModel
                {
                Products = products
                };

            return View(viewModel);
            }

        public async Task<IActionResult> Details(int id)
            {
            var productDto = await _productService.GetByIdAsync(id);

            if (productDto == null)
                {
                return NotFound();
                }

            var viewModel = new ProductDetailsViewModel
                {
                Product = productDto
                };

            return View(viewModel);
            }
        }
    }

