# JWT auth
* Update entity
* Create migration
* Generate DTOs
* Add `System.IdentityModel.Tokens.Jwt` package
* Implement JWT token
* Register endpoint
* Login endpoint


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
        public string username { get; set; }
        public string token { get; set; }
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

```
in `Interfaces/ITokenService.cs`
```cs

```
### Register endpoint
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

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] RegisterDto registerDto)
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

            return user;
        }
    }
}
```
### Login endpoint
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

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] RegisterDto registerDto)
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

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login([FromBody] LoginDto loginDto)
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

            return user;                

        }
    }
}
```
