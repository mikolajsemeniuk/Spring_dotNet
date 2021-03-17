# JWT auth
* Update entity
* Create migration
* Generate DTOs
* Add `System.IdentityModel.Tokens.Jwt` package
* Implement JWT token
* Add JWT service
* Register/Login endpoint
* Authentication middleware

### Update entity
Update `Data/AppUser.cs`
```cs
namespace test.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
```
### Create migration
```sh
dotnet ef migrations add UserPasswordAdded
dotnet ef database update
```
### Generate DTOs
in `DTO/RegisterDto.cs`
```cs
using System.ComponentModel.DataAnnotations;

namespace test.DTO
{
    public class RegisterDto
    {
        [StringLength(50, MinimumLength = 4)]
        public string username { get; set; }
        [StringLength(50, MinimumLength = 4)]
        public string password { get; set; }
    }
}
```
in `DTO/LoginDto.cs`
```cs
namespace test.DTO
{
    public class LoginDto
    {
        [StringLength(50, MinimumLength = 4)]
        public string username { get; set; }
        [StringLength(50, MinimumLength = 4)]
        public string password { get; set; }
    }
}
```
in `DTO/UserDto.cs`
```cs
namespace test.DTO
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
```
### Add `System.IdentityModel.Tokens.Jwt` package
in `test.csproj`
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
  </ItemGroup>
</Project>
```
### Implement JWT token
in `Interfaces/ITokenService.cs`
```cs
using test.Entities;

namespace test.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
```
in `Services/TokenService.cs`
```cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using test.Entities;
using test.Interfaces;

namespace test.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // create the claims 
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // create signingCredentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}
```
in `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=test;User Id=sa;Password=super_secret_password"
  },
  "TokenKey": "this should be 32 random characters string",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
### Add JWT service
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using test.Data;
using test.Services;
using test.Interfaces;

namespace test
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
            services.AddScoped<ITokenService, TokenService>(); // ADD THIS TO ENABLE TOKEN SERVICE AND REMEMBER TO IMPLEMENET INTERFACE TO SERVICE!
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```
### Register/Login endpoint
in `Controllers/AuthController.cs`
```cs
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test.Data;
using test.Entities;
using test.DTO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using test.Interfaces;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly DataContext _context;

        public AuthController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == registerDto.username.ToLower()))
                return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context
                .Users
                .SingleOrDefaultAsync(
                    x => x.UserName == loginDto.username);

            if (user == null)
                return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));

            if(!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Invalid Password");

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
```
### Authentication middleware
** If you do not have `Microsoft.AspNetCore.Authentication.JwtBearer` package in your `test.csproj` you have to add it first but in `.NET5.0` and higher it should be by default
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
  </ItemGroup>
</Project>
```
to use auth schema update `Startup.cs`
```cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using test.Data;
using test.Services;
using test.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer; // ADD THIS TO ENABLE JwtBearerDefaults

namespace test
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
            services.AddScoped<ITokenService, TokenService>(); // ADD THIS TO ENABLE TOKEN SERVICE
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            // services.AddCors();
            
            // ADD THIS
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
            
            // app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

            app.UseAuthentication(); // ADD THIS, IMPORTANT ORDER MATTERS

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```
Update `Controllers/AuthController.cs` to apply auth middleware
** to check if everything work add `Authorization` Header with `Bearer {{ token }}` value to get authenticated
```cs
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test.Data;
using test.Entities;
using test.DTO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using test.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly DataContext _context;

        public AuthController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == registerDto.username.ToLower()))
                return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context
                .Users
                .SingleOrDefaultAsync(
                    x => x.UserName == loginDto.username);

            if (user == null)
                return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));

            if(!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Invalid Password");

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [Authorize]
        [HttpGet("auth")]
        public string Authorize()
        {
            return "only users with token could see this";
        }

        [AllowAnonymous]
        [HttpGet("unauth")]
        public string Annonymous()
        {
            return "users and annonymous could see this";
        }
    }
}
```
