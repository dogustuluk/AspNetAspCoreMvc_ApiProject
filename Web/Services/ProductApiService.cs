using Core.DTOs;

namespace Web.Services
{
    /*
     * httpClient'ı hiç bir zaman kendimiz üretmiyoruz, program.cs içerisinde DI container olarak geçip, container'ın kendisinin nesne örneği üretmesini sağlıyoruz. Bu sayede soket yokluğu gibi hatalardan arındırılmış oluruz, best practise'dir.
     */
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<List<ProductWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<List<ProductWithCategoryDto>>>("products/GetProductsWithCategory");//parantez içerisine products controller'ındaki GetProductsWithCategory adlı metot için istek yapılacağını belirtiyoruz.
            return response.Data;
        }
        public async Task<ProductDto> SaveAsync(ProductDto newProduct)
        {
            var response = await _httpClient.PostAsJsonAsync("products",newProduct);
            if(!response.IsSuccessStatusCode) return null;
            //başarılı ise
            var responseBody = await response.Content.ReadFromJsonAsync<CustomResponseDto<ProductDto>>();
            return responseBody.Data;
        }
        public async Task<bool> UpdateAsync(ProductDto newProduct)
        {
            var response = await _httpClient.PutAsJsonAsync("products", newProduct);
            
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> RemoveAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<CustomResponseDto<ProductDto>>($"products/{id}");
            //if (response.Errors.Any())
            //{
            //    //hata yazdır, loglama yap,...
            //}
            return response.Data;
        }
    }
}
