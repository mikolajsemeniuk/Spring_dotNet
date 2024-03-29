# Template
Initial Template with SQL and Identity

### Create Initial Template
```sh
dotnet new webapi -o template
cd template
```
### Install packages
Version could be different based on dotnet version already installed and packages may change
```sh
# use different package if would like to use PostgreSQL, MySQL or SQLite
# for more information check section 3. Connect to database
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 5.0.0 &&
dotnet add package Microsoft.EntityFrameworkCore.Design -v 5.0.0 &&
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore -v 5.0.0 &&
dotnet add package System.IdentityModel.Tokens.Jwt -v 6.9.0
```
After all your `template.csproj` should looks like this
```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="5.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.6.3" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.9.0" />
  </ItemGroup>

</Project>
```
### Configure appsettings
Configure `appsettings.Development.json` to customize db connection, roles
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=template;User Id=sa;Password=CHANGE_PASSWORD"
  },
  "TokenKey": "super secret key to your token service",
  "Roles": [
    "Admin",
    "Moderator",
    "Member"
  ],
  "DropAndSeedDb": {
    "Enable": true,
    "FileLocation": "Data/SeedData.json"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
### Create Entities
in `Entities/AppUser.cs`
```cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace template.Entities
{
    public class AppUser : IdentityUser<int>
    {
        // Put all properties You want to add
        // to `AspNetUsers` Table in Database
        //
        // example:
        //      public DateTime CreatedAt { get; set; }
        //      public DateTime UpdatedAt { get; set; }



        // Relation below is demanded by
        // `AspNetCore.Identity` to create
        // `AspNetUsers` table in database
        // with columns:
        //      * Id
        //      * UserName
        //      * NormalizedUserName
        //      * Email
        //      * NormalizedEmail
        //      * EmailConfirmed
        //      * PasswordHash
        //      * SecurityStamp
        //      * ConcurrencyStamp
        //      * PhoneNumber
        //      * PhoneNumberConfirmed
        //      * TwoFactorEnabled
        //      * LockoutEnd
        //      * LockoutEnabled
        //      * AccessFailedCount
        //
        // Put other relations you want
        // to include after that
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
```
in `Entities/AppRole.cs`
```cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace template.Entities
{
    public class AppRole : IdentityRole<int>
    {
        // Relation below is demanded by
        // `AspNetCore.Identity` to create
        // `AspNetRoles` table in database
        // with columns:
        //      * Id
        //      * Name
        //      * NormalizedName
        //      * ConcurrencyStamp
        //
        // Put other relations you want
        // to include after that
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
```
in `Entities/AppUserRole.cs`
```cs
using Microsoft.AspNetCore.Identity;

namespace template.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        // Relation below is demanded by
        // `AspNetCore.Identity` to create
        // `AspNetUserRoles` table in database
        // with columns:
        //      * UserId
        //      * RoleId
        //
        // Put other relations you want
        // to include after that
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
```
### Add DbContext
in `Data/DbContext.cs`
```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using template.Entities;

namespace template.Data
{
    // Every derive means one table in database
    //
    // We have to specify all tables because
    // we changed default key from `string` to `int`
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
                               IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                               IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relations below is demanded by
            // AspNetCore.Identity
            // Do not touch them and
            // specify other relations after
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // Relations below is demanded by
            // AspNetCore.Identity
            // Do not touch them and
            // specify other relations after
            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }
    }
}
```
### Create Service
in `Interfaces/ITokenService.cs`
```cs
using System.Threading.Tasks;
using template.Entities;

namespace template.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
```
in `Services/TokenService.cs`
```cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using template.Entities;
using template.Interfaces;

namespace template.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        // MODIFY THIS
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // Specify Payload
            var claims = new List<Claim>()
            {
                // Specify here claim you want
                // to include in payload of token
                // remember you only could include
                // properties which are already defined
                // on Claim Object
                //
                // So far you included 
                //      * nameid = user.Id
                //      * unique_name = user.UserName
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)

            };

            // Add all roles to List of claims
            // every claim will stand for every role
            // user could have
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Specify Signature
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create Token
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // Create TokenHandler and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
```
### Configure services
in `Startup.cs`
```cs
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using template.Data;
using template.Entities;
using template.Interfaces;
using template.Services;

namespace template
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
            // Token Service
            services.AddScoped<ITokenService, TokenService>();
            
            // Db Setup
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });

            // Identity Setup
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

            // Default Auth Schema Setup
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

            // Policy Setup
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin", "Moderator"));
                opt.AddPolicy("RequireModerateRole", policy => policy.RequireRole("Moderator"));
            });
            
            services.AddControllers();
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "template", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "template v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Remeber to add this to enable
            // previously defined auth schema
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
### Configure initial settings
in `Data/Seed.cs`
```cs
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using template.DTO;
using template.Entities;

namespace template.Data
{
    public class Seed
    {
        public static async Task InitSeed(IConfiguration config, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (!bool.Parse(config["DropAndSeedDb:Enable"]))
                return;

            // Remove all current users
            var oldUsers = await userManager.Users.ToListAsync();
            foreach (var user in oldUsers)
                await userManager.DeleteAsync(user);

            // Remove all current roles
            var oldRoles = await roleManager.Roles.ToListAsync();
            foreach (var role in oldRoles)
                await roleManager.DeleteAsync(role);

            // Get roles from config
            var newRoles = config
                .GetSection("Roles")
                .GetChildren()
                .ToArray()
                .Select(c => c.Value)
                .ToArray();

            // Add roles
            foreach (var role in newRoles)
                await roleManager.CreateAsync(new AppRole { Name = role });

            // Read list of users
            var userData = await System.IO.File.ReadAllTextAsync(config["DropAndSeedDb:FileLocation"]);
            var newUsers = JsonSerializer.Deserialize<List<MockDto>>(userData);

            // Add users
            foreach (var user in newUsers)
            { 
                var _user = new AppUser { UserName = user.UserName, Email = user.Email };
                await userManager.CreateAsync(_user, "Excel!1");
                await userManager.AddToRolesAsync(_user, user.Roles.ToArray());
            }
        }
    }
}
```
in `Program.cs`
```cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using template.Data;
using template.Entities;

namespace template
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                var config = services.GetRequiredService<IConfiguration>();

                await context.Database.MigrateAsync();
                await Seed.InitSeed(config, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<Logger<Program>>();
                logger.LogError($"error: {ex}");
            }

            await host.RunAsync();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```
### Adding DTO
in `DTO/RegisterDto.cs`
```cs
using System.ComponentModel.DataAnnotations;

namespace template.DTO
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [StringLength(50, MinimumLength = 4)]
        public string UserName { get; set; }
        [StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
```
in `DTO/LoginDto.cs`
```cs
using System.ComponentModel.DataAnnotations;

namespace template.DTO
{
    public class LoginDto
    {
        [StringLength(50, MinimumLength = 4)]
        public string UserName { get; set; }
        [StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
```
in `DTO/UserDto.cs`
```cs
namespace template.DTO
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
```
in `DTO/MockDto.cs`
```cs
namespace template.DTO
{
    public class MockDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
```
### Create controller
in `Controllers/AuthController.cs`
```cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using template.DTO;
using template.Entities;
using template.Interfaces;

namespace template.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email.ToLower()))
                return BadRequest("Email is taken");

            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName.ToLower()))
                return BadRequest("Username is taken");

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                Email = registerDto.Email.ToLower()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRolesAsync(user, new[] { "Member" });
            if (!roleResult.Succeeded)
                return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager
                .Users
                .SingleOrDefaultAsync(
                    x => x.UserName == loginDto.UserName);

            if (user == null)
                return Unauthorized("Invalid Username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Invalid password");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user)
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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin")]
        public string AuthorizeWithAdminPolicy()
        {
            return "only admin could see this";
        }

        [Authorize(Policy = "RequireModerateRole")]
        [HttpGet("mod")]
        public string AuthorizeWithModeratorPolicy()
        {
            return "only moderator could see this";
        }
    }
}
```
### Create migration and run
```sh
cd template &&
dotnet ef migrations add InitialCreate -o Data/Migrations &&
dotnet ef database update &&
dotnet run
```
