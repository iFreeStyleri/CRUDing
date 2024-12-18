using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Products;
using CRUDing.Domain.Entities;

namespace CRUDing.Core.Abstractions.Services
{
    public interface IProductService
    {
        public Task<IBaseResponse<object>> AddProduct(AddProductDTO product);
        public Task<IBaseResponse<Product>> GetProduct(int productId);
        public Task<IBaseResponse<List<Product>>> GetOffsetProduct(int page);
        public Task<IBaseResponse<object>> RemoveProduct(int productId);
        public Task<IBaseResponse<Product>> UpdateProduct(UpdateProductDTO product);
        public Task<IBaseResponse<List<Product>>> GetOffsetProductByCategory(int page, string category);


    }
}
