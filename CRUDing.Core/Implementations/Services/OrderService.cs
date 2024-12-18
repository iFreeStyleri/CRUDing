using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Responses.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CRUDing.Core.Implementations.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<CartProduct> _cartProductRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IBaseRepository<Order> orderRepository,
            IBaseRepository<User> userRepository,
            IBaseRepository<CartProduct> cartProductRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _cartProductRepository = cartProductRepository;
            _logger = logger;
        }

        public async Task<IBaseResponse<List<GetOffsetOrderResponse>>> GetOffsetOrders(int page, string userEmail)
        {
            try
            {
                _logger.LogInformation("Получение заказов на странице {page} пользователя {userEmail}...", page, userEmail);
                var orders = await _orderRepository.GetAll()
                    .Include(i => i.User)
                    .Include(i => i.Products)
                    .Where(w => !w.IsCompleted && w.User.Email == userEmail).OrderBy(o => o.Id)
                    .Select(s => new GetOffsetOrderResponse(s)).Skip(page * 10).Take(10).ToListAsync();
                if (orders.Count == 0)
                    return new BaseResponse<List<GetOffsetOrderResponse>>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Orders not found"
                    };
                _logger.LogInformation("Получено заказов на странице {page} пользователя {userEmail} в количестве {orders.Count}", page, userEmail, orders.Count);
                return new BaseResponse<List<GetOffsetOrderResponse>>
                {
                    Code = HttpStatusCode.OK,
                    Data = orders
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<List<GetOffsetOrderResponse>>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<GetOffsetOrderResponse>>> GetOffsetCompletedOrders(int page, string userEmail)
        {
            try
            {
                _logger.LogInformation("Получение завершённых заказов на странице {page} пользователя {userEmail}...", page, userEmail);
                var orders = await _orderRepository.GetAll()
                    .Include(i => i.User)
                    .Include(i => i.Products)
                    .Where(w => w.IsCompleted && w.User.Email == userEmail).OrderBy(o => o.Id).Select(s => new GetOffsetOrderResponse(s)).Skip(page * 10).Take(10).ToListAsync();
                if (orders.Count == 0)
                {
                    _logger.LogWarning("Заказы на странице {page} у пользователя {userEmail} не найдены", page, userEmail);
                    return new BaseResponse<List<GetOffsetOrderResponse>>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Orders not found"
                    };
                }
                _logger.LogInformation("Получено заказов на странице {page} пользователя {userEmail} в количестве {count}", page, userEmail, orders.Count);
                return new BaseResponse<List<GetOffsetOrderResponse>>
                {
                    Code = HttpStatusCode.OK,
                    Data = orders
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<List<GetOffsetOrderResponse>>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Order>> CreateOrder(string userEmail)
        {
            try
            {
                _logger.LogInformation("Создание заказа для пользователя {userEmail}...", userEmail);
                var user = await _userRepository.GetAll().SingleOrDefaultAsync(s => s.Email == userEmail && !s.IsDeleted);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь {userEmail} не найден", userEmail);
                    return new BaseResponse<Order>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }
                var productsOfCart = await _cartProductRepository.GetAll()
                    .Include(i => i.Cart)
                    .ThenInclude(i => i.User)
                    .Include(i => i.Product)
                    .Where(w => w.Cart.User.Email == userEmail).ToListAsync();
                if (productsOfCart.Count == 0)
                {
                    _logger.LogWarning("Продуктов в корзине {userEmail} не найдены", userEmail);
                    return new BaseResponse<Order>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Products not found"
                    };
                }
                _logger.LogInformation("Продуктов в корзине {userEmail} найдено {count}", userEmail, productsOfCart.Count);
                var order = new Order{Created = DateTime.UtcNow};
                order.Products = new List<ProductItem>();
                productsOfCart.ForEach(f => order.Products.Add(new ProductItem
                {
                    Count = f.Count,
                    Product = f.Product,
                    CurrentCost = new Money(f.Product.Cost.Currency, f.Product.Cost.Value)
                }));
                order.User = user;
                _orderRepository.Add(order);
                _cartProductRepository.RemoveRange(productsOfCart);
                await _orderRepository.SaveChangesAsync();
                _logger.LogInformation("Продукты в корзине {userEmail} очищены и добавлены в заказ {id}", userEmail, order.Id);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Order>> ChangeStatusOrder(int orderId, bool isCompleted)
        {
            try
            {
                _logger.LogInformation("Обновление статуса заказа {orderId} на статус: {isCompleted}...", orderId, isCompleted);
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {orderId} не найден", orderId);
                    return new BaseResponse<Order>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Order not found"
                    };
                }
                order.IsCompleted = isCompleted;
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Статус заказа {orderId} обновлён на статус: {isCompleted}", orderId, isCompleted);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<GetOrderWithProductsResponse>> GetOrderWithProducts(int orderId, string userEmail)
        {
            try
            {
                _logger.LogInformation("Получение заказа {orderId} пользователя {userEmail}", orderId, userEmail);
                var order = await _orderRepository.GetAll()
                    .Include(i => i.User)
                    .Include(i => i.Products)
                    .SingleOrDefaultAsync(s => s.Id == orderId && s.User.Email == userEmail);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {orderId} у пользователя {userEmail} не найден", orderId, userEmail);
                    return new BaseResponse<GetOrderWithProductsResponse>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "Order not found"
                    };
                }
                return new BaseResponse<GetOrderWithProductsResponse>
                {
                    Code = HttpStatusCode.OK,
                    Data = new GetOrderWithProductsResponse(order)
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<GetOrderWithProductsResponse>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Order>> RemoveOrder(int orderId)
        {
            try
            {
                _logger.LogInformation("Получение заказа {orderId} для удаления...", orderId);
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {orderId} не найден", orderId);
                    return new BaseResponse<Order>
                    {

                        Code = HttpStatusCode.NotFound,
                        Message = "Order not found"
                    };
                }
                order.IsDeleted = true;
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Заказ {orderId} удалён", orderId);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<Order>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
