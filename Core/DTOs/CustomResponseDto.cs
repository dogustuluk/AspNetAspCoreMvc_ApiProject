using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTOs
{//bu api'lere özgü bir durumdur. Mvc için gerekmez.
    public class CustomResponseDto<T>
    {
        /*
         * bir class'ın içerisinde static ve geriye de yeni bir instance dönen metotlar var ise; bunlara ->> static factory method denmektedir.
         * bu durum factory design pattern'den gelmektedir. bu patternde ayrı sınıflar ve interface'ler oluşturmak yerine direkt olarak hangi sınıfı dönmek istiyorsak o sınıfın içeriisnde static metotlar tanımlayarak geriye instance'lar döneriz. Yani new anahtar sözcüğünü kullanmak yerine direkt olarak bu metotları kullanarak nesne üretme olayını bu sınıf içerisinde geliştirmekteyiz.
         * static factory method design pattern olarak adlandırılır.
         */
        public T Data { get; set; }
        [JsonIgnore]//body'sinde statusCode dönmek istemiyoruz ama kod içerisinde lazım ise.
        public int StatusCode { get; set; }//client'lara dönmek istemiyorum bunu.
        public List<String> Errors { get; set; }
        public static CustomResponseDto<T> Success(int statusCode, T data)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Data = data };
        }
        public static CustomResponseDto<T> Success(int statusCode)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode };
        }
        public static CustomResponseDto<T> Fail(int statusCode,List<string> errors)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = errors };
        }
        public static CustomResponseDto<T> Fail(int statusCode, string error)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = new List<string> { error } };
        }

    }
}
