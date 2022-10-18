using AutoMapper;
using Core;
using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _services;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public ProductsController(IProductService services, ICategoryService categoryService, IMapper mapper)
        {
            _services = services;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            
            return View(await _services.GetProductsWithCategory()); 
        }
        
        public async Task<IActionResult> Save()
        {
            var categories = await _categoryService.GetAllAsync();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");//id'yi seç, kullanıcıya name'i göster
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {

            if (ModelState.IsValid)
            {
                await _services.AddAsync(_mapper.Map<Product>(productDto));
                return RedirectToAction(nameof(Index));
            }
            else
            {

                var categories = await _categoryService.GetAllAsync();
                var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());
                ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");//id'yi seç, kullanıcıya name'i göster
                return View();
            }
        }
    }
}
