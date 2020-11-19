using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class DependencyInjector
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            ResolveContext(services, configuration);
            ResolveServices(services);
            ResolveRepositories(services);
            ResolveAdditionals(services, configuration);
        }

        private static void ResolveContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreContext>(x =>
            {
                x.UseMySql(configuration.GetConnectionString("DefaultConnection"));
            });
        }

        private static void ResolveRepositories(IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        private static void ResolveServices(IServiceCollection services)
        {
            
        }

        private static void ResolveAdditionals(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddControllers();
            services.AddHttpClient();
        }
    }
}
