using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
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

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddControllers();
            services.AddHttpClient();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                                              .Where(e => e.Value.Errors.Any())
                                              .SelectMany(x => x.Value.Errors)
                                              .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkiNetAPI", Version = "V1" });
            }); 
        }
    }
}
