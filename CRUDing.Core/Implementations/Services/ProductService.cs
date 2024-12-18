using AutoMapper;
using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Products;
using CRUDing.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CRUDing.Core.Implementations.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IValidator<AddProductDTO> _addProductValidator;
        private readonly IValidator<UpdateProductDTO> _updateProductValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IBaseRepository<Product> productRepository, 
            IBaseRepository<Category> categoryRepository,
            IValidator<AddProductDTO> addProductValidator,
            IValidator<UpdateProductDTO> updateProductValidator,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _addProductValidator = addProductValidator;
            _updateProductValidator = updateProductValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IBaseResponse<object>> AddProduct(AddProductDTO dto)
        {
            try
            {
                _logger.LogInformation("Добавление продукта {@dto}...", dto);
                var validResult = await _addProductValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {@errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<object>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };
                }
                var product = _mapper.Map<Product>(dto);
                var categoryResult = await _categoryRepository.GetByIdAsync(product.Category.Id);
                if (categoryResult == null)
                {
                    _logger.LogWarning("Категория не найдена {@dto}", dto);
                    return new BaseResponse<object>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Category not found"
                    };

                }
                product.Category = categoryResult;
                await _productRepository.AddAsync(product);
                _logger.LogInformation("Продукт добавлен {@dto}", dto);
                return new BaseResponse<object>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<object>
                {
                    Code = HttpStatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<Product>> GetProduct(int productId)
        {
            try
            {
                _logger.LogInformation($"Получение продукта с Id: {productId}");
                var result = await _productRepository.GetByIdAsync(productId);
                if (result == null)
                {
                    _logger.LogInformation("Продукт с Id: {productId} не найден", productId);
                    return new BaseResponse<Product>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Product not found"
                    };
                }
                _logger.LogInformation("Продукт с Id: {productId} получен", productId);
                return new BaseResponse<Product>
                {
                    Code = HttpStatusCode.OK,
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Product>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<Product>>> GetOffsetProduct(int page)
        {
            try
            {
                _logger.LogInformation("Поиск товаров на {page} странице", page);
                var result = await _productRepository.GetAll()
                    .Where(w => !w.IsDeleted).OrderBy(o => o.Id)
                    .Skip(page * 25).Take(25).ToListAsync();
                if (result.Count == 0)
                {
                    _logger.LogWarning("Товары на {page} странице не найдены", page);
                    return new BaseResponse<List<Product>>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Products not found"
                    };
                }
                return new BaseResponse<List<Product>>
                {
                    Code = HttpStatusCode.OK,
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<List<Product>>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<Product>>> GetOffsetProductByCategory(int page, string category)
        {
            try
            {
                _logger.LogInformation("Поиск товаров на {page} странице в категории {category}", page, category);
                var result = await _productRepository.GetAll().Include(i => i.Category)
                    .Where(w => !w.IsDeleted)
                    .Where(w => w.Category.Name == category).OrderBy(g => g.Id)
                    .Skip(page * 25).Take(25).ToListAsync();
                result.ForEach(f => f.Category = null);
                if (result.Count == 0)
                {
                    _logger.LogWarning("Товары на {page} странице в категории {category} не найдены", page, category);
                    return new BaseResponse<List<Product>>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Products not found"
                    };
                }
                _logger.LogInformation($"Найдено {result.Count}");
                return new BaseResponse<List<Product>>
                {
                    Code = HttpStatusCode.OK,
                    Data = result
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<List<Product>>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<object>> RemoveProduct(int productId)
        {
            try
            {
                _logger.LogInformation("Удаление {productId} товара...", productId);
                var result = await _productRepository.GetByIdAsync(productId);
                if (result == null)
                {
                    _logger.LogWarning("Товар {productId} не найден", productId);
                    return new BaseResponse<object>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Product not found"
                    };
                }
                result.IsDeleted = true;
                await _productRepository.UpdateAsync(result);
                _logger.LogInformation("Товар {productId} удалён", productId);
                return new BaseResponse<object>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<object>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Product>> UpdateProduct(UpdateProductDTO dto)
        {
            try
            {
                _logger.LogInformation("Обновление товара {@dto}...", dto);
                var validResult = await _updateProductValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {@errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<Product>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };
                }
                var product = _mapper.Map<Product>(dto);
                _logger.LogInformation("Получение обновляемого товара: {@dto}", dto);
                var updater = await _productRepository.GetByIdAsync(product.Id);
                if (updater == null)
                {
                    _logger.LogWarning("Продукт не найден: {@dto}", dto);
                    return new BaseResponse<Product>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Product not found"
                    };
                }

                updater.Name = product.Name;
                updater.Description = product.Description;
                var category = await _categoryRepository.GetByIdAsync(product.Category.Id);
                if (category != null)
                    updater.Category = category;
                updater.Cost = product.Cost;
                await _productRepository.UpdateAsync(updater);
                _logger.LogWarning("Продукт обновлён: {@updater}", updater);
                return new BaseResponse<Product>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Product>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
