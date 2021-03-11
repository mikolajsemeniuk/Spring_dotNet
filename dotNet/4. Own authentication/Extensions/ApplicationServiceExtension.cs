using api.Data;
using api.Interfaces;
using api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // =================================================
            //
            //  Specify Interface
            // =================================================
            //  While registering the `TokenService` we specify
            //  `ITokenService` first and `TokenService` as
            //  as second argument to be able to:
            //
            //  *   To Inject `ITokenService` instead of 
            //      `TokenService` because it's easier to 
            //      test interface instead of service
            //
            // =================================================
            services.AddScoped<ITokenService, TokenService>();

            // =================================================
            //
            //  Db connection
            // =================================================
            //  While registering the Db connection we have to
            //  import `DataContext` class and use `UseSqlServer`
            //  which comes from `Microsoft.EntityFrameworkCore`
            //  and then use `DefaultConnection` which comes
            //  from `api/appsettings.json`
            //
            // =================================================
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // =================================================
            //
            //  Return services
            // =================================================
            //  Return services to `api/Startup.cs` to
            //  `ConfigureServices` method which is responsible
            //  for inject services to classes
            //
            // =================================================
            return services;
        }
    }
}