using AutoMapper;
using Core;
using Core.DTOs;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ProductServiceCaching : Service<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductServiceCaching(IUnitOfWork unitOfWork, IGenericRepository<Product> repository, IMapper mapper, IProductRepository productRepository) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<List<ProductWithCategoryDto>> GetProductsWithCategory()
        {
            //data al
            var product = await _productRepository.GetProductsWithCategory();
            //product bir list geri döner fakat metot dto beklediği için mapper ile dönüşüm yapılır.
            var productsDto = _mapper.Map<List<ProductWithCategoryDto>>(product);
            //return CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsDto);
            return productsDto;
        }
    }
}
