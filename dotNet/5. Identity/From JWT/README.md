# Identity (From JWT)
* Add package
* Update entities
* Update `DataContext` class
* Configure services

### Add package
Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` to your `csproj` if you do not have it
```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.0" NoWarn="NU1605"/>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.0" NoWarn="NU1605"/>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.6.3"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.0"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.0"/>
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.9.0"/>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="5.0.0"/>
  </ItemGroup>
</Project>
```
### Update entities
in `Entities/AppUser.cs`
```cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Entities
{
    public class AppUser : IdentityUser<int>
    {
        // put any additonal columns you would like to add below
        public DateTime CreatedAt { get; set; }
        
        // put any relations below
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
```
in `Entities/AppRole.cs`
```cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
```
in `Entities/AppUserRole.cs`
```cs
using Microsoft.AspNetCore.Identity;

namespace api.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
```
### Update `DataContext` class
in `Data/DataContext.cs`
```cs
using api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    // every derive means one table in database
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
                               IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                               IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // specify te relations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }
    }
}
```
### Configure services
in `Startup.cs` class
```cs

using System.Text;
using api.Data;
using api.Entities;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace api
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            // ADD THIS TO SERVICE CLASS
            services.AddIdentityCore<AppUser>(opt => 
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "test", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "test v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```
