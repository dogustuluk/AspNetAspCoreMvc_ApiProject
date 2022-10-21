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
    }
}
