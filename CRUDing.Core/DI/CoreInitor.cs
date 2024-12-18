using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Core.Implementations.Services;
using CRUDing.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDing.Core.DI
{
    public static class CoreInitor
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
