using AutoMapper;
using Core;
using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{//business kod bulundurma, service katmanı yapıyor onları
    public class ProductsController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public ProductsController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _service = productService;
        }
        [HttpGet("[action]")] //Get api/products/GetProductsWithCategory -> get metodunun içerisine metot ismini yazmamız gerekir çünkü aynı metoda(get) sahip olduğundan framework hangisinin çağrılacağını bilemez ve hata verir. Bu çakışmanın önüne geçmek için kullanılır. action vererek metot adını direkt almasını sağlar.
        public async Task<IActionResult> GetProductsWithCategory()
        {
            return CreateActionResult(await _service.GetProductsWithCategory());
            /*custom repository olmayanlar
             *             var products = await _service.GetAllAsync();
             *             var productsDto = _mapper.Map<List<ProductDto>>(products).ToList();
             *bu iki satırı yazmak zorundayız çünkü generic olduğundan herhangi bir mapleme işlemi yapamıyoruz. özelleştirilmiş bir repo ve servisimiz var ise direkt api'nin istediği datayı tek satırda dönebiliyoruz. bu fazlalık olan iki satırı ise olması gereken yer olan Service katmanında yazıyoruz.
             */
        }

        [HttpGet] //Get api/products
        public async Task<IActionResult> All()
        {
            var products = await _service.GetAllAsync();
            //products bir entity'dir, geriye dto dönmemiz gerekmektedir.
            var productsDto = _mapper.Map<List<ProductDto>>(products).ToList();//products IEnumerable döndüğü için list'e dönüştür.
            /*return OK (CustomResponseDto<List<ProductDto>>.Success(200,productsDto));
             * aşağıdaki gibi custom bir base controller oluşturup, orada tanımlamış olduğumuz metot ile geri dönüş sağlarsak hem best practise olur hem de her defasında status code belirtip ek olarak da Ok durumunu vermemiş oluruz ve daha okunaklı bir yapı ortaya çıkar.
             */
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDto));
        }
        [HttpGet("{id}")]
        /*
         * eğer üstte id'yi belirtirsek metot parametresindeki id, bunu query string'ten bekler fakat burada yazarsak url üzerinden de alır. -> www.mysite.com./api/products/5 
         * query string olursa -> Get/www.mysite.com.-api de yazabiliriz-/products?5
         * get içerisinde parametre verirsek -> www.mysite.com.-api de yazabiliriz-/api/products/5 
         */
        public async Task<IActionResult> GetById(int id)//
        {
            var product = await _service.GetByIdAsync(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productDto));
        }
        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDTO)
        {
            var product = await _service.AddAsync(_mapper.Map<Product>(productDTO));
            var productDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productDto));
        }
        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productUpdateDto)//
        {
            await _service.UpdateAsync(_mapper.Map<Product>(productUpdateDto));
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)//
        {
            var product = await _service.GetByIdAsync(id);
            await _service.RemoveAsync(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }
    }
}
