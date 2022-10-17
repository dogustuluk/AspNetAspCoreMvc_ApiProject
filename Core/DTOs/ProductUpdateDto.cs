using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ProductUpdateDto
    {//best practise açısından update için ek olarak dto açmak doğru değildir, olabildiğince ortak kullanmaya bakmalıyız. 
        //api->dto = mvc->viewModel
        //her bir sayfaya özgü dto-viewModel olmalıdır.
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
