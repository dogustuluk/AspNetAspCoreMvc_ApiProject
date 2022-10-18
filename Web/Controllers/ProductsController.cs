using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {//cache datası alınacak.
        private readonly IProductService _services;

        public ProductsController(IProductService services)
        {
            _services = services;
        }

        public async Task<IActionResult> Index()
        {
            //sadece bir web uygulaması ise customResponse dönmeye gerek yoktur. burada service katmanını değiştirmek istemediğimiz için yapıyoruz.
            var CustomResponse = await _services.GetProductsWithCategory();
            //return View(CustomResponse.Data); api
            return View(CustomResponse); 
        }
    }
}
