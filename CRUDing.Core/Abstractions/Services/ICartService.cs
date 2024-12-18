using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Responses.Cart;

namespace CRUDing.Core.Abstractions.Services
{
    public interface ICartService
    {
        public Task<IBaseResponse<GetCartProductResponse>> GetCartsOffset(int page, string userEmail);
        public Task<IBaseResponse<CartProduct>> AddProductCart(int productId, string userEmail);
        public Task<IBaseResponse<CartProduct>> RemoveProductCart(int productId, string userEmail);
        public Task<IBaseResponse<CartProduct>> ClearProductsOfCart(string userEmail);
        public Task<IBaseResponse<CartProduct>> UpdateProductOfCart(int productId, int count ,string userEmail);
    }
}
