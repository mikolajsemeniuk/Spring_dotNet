using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // =================================================
            //
            //  Add authentication
            // =================================================
            //  Add default authentication scheme to use
            //  [AllowAnnonymous] and [Auth] in controller
            //  without this service we would get an error
            //  to use it we need to add package
            //  `Microsoft.AspNetCore.Authentication.JwtBearer`
            //
            // =================================================
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
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