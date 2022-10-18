using AutoMapper;
using Core;
using Core.DTOs;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caching
{
    /*
     * cache adayı nasıl olmalı -> cache'lenecek data çok sık olarak değiştirilmeyen ama çok sık ihtiyaç duyduğumuz data olmalıdır.
     * varolan yapıyı bozmamak için controller'larda hangi class için yapıyorsak(burada Product) implemente ettiği interface'i burada da implemente etmemiz gerekir.
     * buna decorator design pattern veya yakın olan proxy pattern'inin implamantasyonunu gerçekleştiriyoruz
     * open-closed prensiplerini uyguluyoruz burada. Yani değişime kapalı ama değişime açık bir halde bir yapı var.
     */
    public class ProductServiceWithCaching : IProductService
    {
        //tüm productları tutacak bir key tut.
        private const string CacheProductKey = "productsCache";
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IUnitOfWork unitOfWork, IProductRepository repository, IMemoryCache memoryCache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _memoryCache = memoryCache;
            _mapper = mapper;
            //ilk nesne oluşturulduğunda bir cache'leme yapmamız gerekir. Uygulama ilk ayağa kalktığında cache'te yok ise oluşturacak.
            if(!_memoryCache.TryGetValue(CacheProductKey, out _))//cache'deki datayı almamak için alt tire yazarız.
            {
                _memoryCache.Set(CacheProductKey, _repository.GetProductsWithCategory().Result);
            }
        }

        public async Task<Product> AddAsync(Product entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            //cache'leme işlemi
            await CacheAllProductsAsync();
            return entity;
        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            //cache'leme işlemi
            await CacheAllProductsAsync();
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {//direkt olarak cache'i döndük.
            return Task.FromResult(_memoryCache.Get<IEnumerable<Product>>(CacheProductKey));
        }

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);
            if(product == null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }
            return Task.FromResult(product);
        }

        public Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory()
        {//cache'lemede bu metot nadir kullanılıyorsa eğer direkt olarak repo'dan dönebiliriz, cache'den almasın.
            //direkt cache'den döndük ama bu metot dto ve custom response istediği için ilgili işlemleri yaptık.
            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);
            return Task.FromResult (CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsWithCategoryDto));
        }

        public async Task RemoveAsync(Product entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {//artık cache üzerinde sorgulama yapılmalı.
            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }
        //cache'i yenilemek için ayrı ayrı yazmamak için bir metot yazıyoruz.
        //her çağırdığımızda sıfırdan datayı çekip cache'leme işlemi yapıyor.
        public async Task CacheAllProductsAsync()
        {
           await _memoryCache.Set(CacheProductKey, _repository.GetAll().ToListAsync());
        }
    }
}
