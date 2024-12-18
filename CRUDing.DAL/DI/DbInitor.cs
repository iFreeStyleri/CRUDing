using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Core.Abstractions.Repositories;
using CRUDing.DAL.Repositories;
using CRUDing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDing.DAL.DI
{
    public static class DbInitor
    {
        public static IServiceCollection ConfigureDAL(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<CRUDContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("PgSql"));
                opt.UseTriggers(triggersConfig => triggersConfig.AddAssemblyTriggers(ServiceLifetime.Scoped));
            });
            services.AddTransient<IBaseRepository<Product>, BaseRepository<Product>>();
            services.AddTransient<IBaseRepository<Category>, BaseRepository<Category>>();
            services.AddTransient<IBaseRepository<User>, BaseRepository<User>>();
            services.AddTransient<IBaseRepository<Cart>, BaseRepository<Cart>>();
            services.AddTransient<IBaseRepository<CartProduct>, BaseRepository<CartProduct>>();
            services.AddTransient<IBaseRepository<Order>, BaseRepository<Order>>();
            services.AddTransient<IBaseRepository<ProductItem>, BaseRepository<ProductItem>>();

            return services;
        }
    }
}
