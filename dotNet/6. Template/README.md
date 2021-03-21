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
dotnet add package System.IdentityModel.Tokens.Jwt -v 6.9.0 &&
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore -v 5.0.0
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
    "DefaultConnection": "Server=127.0.0.1;Database=template;User Id=sa;Password=super_secret_password"
  },
  "TokenKey": "super secret key to your token service",
  "Roles": [
    "Admin",
    "Moderator",
    "Member"
  ],
  "Admin": {
    "Enable": true,
    "Data": {
      "Email": "admin@dev.com",
      "Username": "admin",
      "Password": "Semafor4!"
    }
  },
  "DropAndSeedDb": {
    "Enable": true,
    "FileLocation": ""
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
