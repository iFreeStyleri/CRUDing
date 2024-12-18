using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Responses.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CRUDing.Core.Implementations.Services
{
    public class CartService : ICartService
    {
        private IBaseRepository<CartProduct> _cartProductRepository;
        private IBaseRepository<Cart> _cartRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(IBaseRepository<CartProduct> cartProductRepository, IBaseRepository<Cart> cartRepository, ILogger<CartService> logger)
        {
            _cartProductRepository = cartProductRepository;
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<IBaseResponse<GetCartProductResponse>> GetCartsOffset(int page, string userEmail)
        {
            try
            {
                _logger.LogInformation("Получение корзины {userEmail} на странице {page}...", userEmail, page);
                var cartProducts = await _cartProductRepository.GetAll()
                    .Include(s => s.Cart)
                    .ThenInclude(i => i.User)
                    .Include(i => i.Product)
                    .Where(w => w.Cart.User.Email == userEmail).OrderBy(s => s.Id)
                    .Skip(page * 10).Take(10).ToListAsync();

                if (cartProducts.Count == 0)
                {
                    _logger.LogWarning("Корзина пользователя {userEmail} на странице {page} не найдена", userEmail, page);
                    return new BaseResponse<GetCartProductResponse>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Cart is empty"
                    };
                }
                var costCart = await _cartProductRepository.GetAll()
                    .Include(s => s.Cart)
                    .ThenInclude(i => i.User)
                    .Where(w => w.Cart.User.Email == userEmail).OrderBy(s => s.Id).Select(s => s.Product.Cost.Value * s.Count)
                    .SumAsync(s => s);
                _logger.LogInformation("Получена стоимость корзины: {costCart}", costCart);
                var data = new GetCartProductResponse(cartProducts, new Money(cartProducts.First().Product.Cost.Currency, costCart));
                return new BaseResponse<GetCartProductResponse>
                {
                    Code = HttpStatusCode.OK,
                    Data = data
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<GetCartProductResponse>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<CartProduct>> AddProductCart(int productId, string userEmail)
        {
            try
            {
                _logger.LogInformation("Добавление продукта {productId} в корзину {userEmail}...", productId, userEmail);
                var cartProduct = await _cartProductRepository.GetAll()
                    .Include(s => s.Cart)
                    .ThenInclude(i => i.User)
                    .Include(i => i.Product)
                    .Where(w => w.Cart.User.Email == userEmail).SingleOrDefaultAsync(s => s.ProductId == productId);
                if (cartProduct != null)
                {
                    _logger.LogWarning("В корзине {userEmail} уже есть продукт {productId}", userEmail, productId);
                    return new BaseResponse<CartProduct>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = "This product has already been added to the cart"
                    };
                }
                var cart = await _cartRepository.GetAll().Include(i => i.User).SingleOrDefaultAsync(w => w.User.Email == userEmail);
                var newCartProduct = new CartProduct
                {
                    Cart = cart,
                    Count = 1,
                    ProductId = productId
                };
                await _cartProductRepository.AddAsync(newCartProduct);
                _logger.LogWarning("В корзину {userEmail} добавлен продукт {productId}", userEmail, productId);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.OK,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<CartProduct>> RemoveProductCart(int productId, string userEmail)
        {
            try
            {
                _logger.LogInformation("Удаление продукта {productId} из корзины {userEmail}...", productId, userEmail);
                var cartProduct = await _cartProductRepository.GetAll()
                    .Include(s => s.Cart)
                    .ThenInclude(i => i.User)
                    .Include(i => i.Product)
                    .Where(w => w.Cart.User.Email == userEmail)
                    .SingleOrDefaultAsync(s => s.ProductId == productId);
                if (cartProduct == null)
                {
                    _logger.LogInformation("Продукт {productId} в корзине {userEmail} не найден", productId, userEmail);
                    return new BaseResponse<CartProduct>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Product of cart not found"
                    };
                }
                await _cartProductRepository.RemoveAsync(cartProduct);
                _logger.LogInformation("Продукт {productId} в корзине {userEmail} удалён", productId, userEmail);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }

        }

        public async Task<IBaseResponse<CartProduct>> ClearProductsOfCart(string userEmail)
        {
            try
            {
                _logger.LogInformation("Очистка корзины {userEmail}...");
                var cartProducts = await _cartProductRepository.GetAll()
                    .Include(i => i.Cart)
                    .ThenInclude(i => i.User).Where(w => w.Cart.User.Email == userEmail)
                    .Select(s => new CartProduct { Id = s.Id }).ToListAsync();
                if (cartProducts.Count == 0)
                {
                    _logger.LogWarning("В корзине {userEmail} продукты не найдены", userEmail);
                    return new BaseResponse<CartProduct>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Cart is empty"
                    };
                }
                await _cartProductRepository.RemoveRangeAsync(cartProducts);
                _logger.LogInformation("Корзина {userEmail} очищена", userEmail);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<CartProduct>> UpdateProductOfCart(int productId, int count, string userEmail)
        {
            try
            {
                _logger.LogInformation("Обновление товара {productId} у пользователя {userEmail}", productId, userEmail);
                var cartProduct = await _cartProductRepository.GetAll()
                    .Include(s => s.Product)
                    .Include(s => s.Cart).ThenInclude(s => s.User)
                    .SingleOrDefaultAsync(s => s.Cart.User.Email == userEmail && s.ProductId == productId);
                if (cartProduct == null)
                {
                    _logger.LogWarning("Продукт {productId} в корзине {userEmail} не найден", productId, cartProduct);
                    return new BaseResponse<CartProduct>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Product of cart not found"
                    };
                }
                cartProduct.Count = count;
                await _cartProductRepository.UpdateAsync(cartProduct);
                _logger.LogInformation("Продукт {productId} в корзине {userEmail} обновлён на {count} количество", productId, userEmail, count);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<CartProduct>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
