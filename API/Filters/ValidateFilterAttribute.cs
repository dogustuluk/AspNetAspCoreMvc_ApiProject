using Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class ValidateFilterAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //fluent validation kullanmasak da validation hatalarını görmek için modelState.isValid üzerinden hataları görebiliriz.
            if(!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();//Values ile modelStateDictionary geliyor ama biz tek tek ele almak istediğimiz için selectMany diyoruz. selectMany flat yapar, yani tek bir property'i almamıza imkan sağlar.
                context.Result = new BadRequestObjectResult(CustomResponseDto<NoContentDto>.Fail(400,errors));//ObjectResult seçiyoruz çünkü, response'ın body'sinde hata mesajları da göndermek istiyorum.

            }
        }
    }
}
