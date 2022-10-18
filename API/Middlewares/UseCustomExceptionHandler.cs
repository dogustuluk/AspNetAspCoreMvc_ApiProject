using Core.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Service.Exceptions;
using System.Text.Json;

namespace API.Middlewares
{
    public static class UseCustomExceptionHandler
    {
        //this ile hangi tip olduğunu belirtmemiz lazım. Bizim tipimiz web app, eğer buna gidersek IApplicationBuilder'ı implemente ettiğinden dolayı bunu implemente etmiş olan tüm class'larda kullanabiliriz. O yüzden bunun için bir metot yazıyoruz.
        public static void UseCustomException(this IApplicationBuilder app)
        {
            //api uygulaması olduğu için geriye json dönüyoruz.
            app.UseExceptionHandler(config =>
            {
                //Run-> sonlandırıcı bir middleware'dir(terminal middleware). yani request buraya girdiği anda daha ileriye gitmeyecek(controller'lara, metotlara kadar gitmez), geriye dönecektir.
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();//bu interface üzerinden uygulamada fırlatılan hataları alırız.
                    var statusCode = exceptionFeature.Error switch
                    {
                        ClientSideException => 400,
                        _ => 500
                    };
                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode,exceptionFeature.Error.Message);//bu bir tip, geriye dönmek için json'a serialize etmek lazım. Custom middleware olduğu için, controller'lardaki gibi otomatik olarak json'a dönüştürme olmuyor
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    //bu middleware'i aktif hale getirmek için program.cs tarafında da belirtmemiz lazım.
                });
            });
        }
    }
}
