using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Responses.Order;
using Microsoft.AspNetCore.Mvc;

namespace CRUDing.Core.Abstractions.Services
{
    public interface IOrderService
    {
        Task<IBaseResponse<List<GetOffsetOrderResponse>>> GetOffsetOrders(int page, string userEmail);
        Task<IBaseResponse<List<GetOffsetOrderResponse>>> GetOffsetCompletedOrders(int page, string userEmail);
        Task<IBaseResponse<Order>> CreateOrder(string userName);
        Task<IBaseResponse<Order>> ChangeStatusOrder(int orderId, bool isCompleted);
        Task<IBaseResponse<GetOrderWithProductsResponse>> GetOrderWithProducts(int id, string userName);
        Task<IBaseResponse<Order>> RemoveOrder(int orderId);
    }
}
