# JWT auth
* Update entity
* Create migration
* Generate DTOs
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
### Register endpoint
in `Controllers/AuthController.cs`
```cs

```
### Login endpoint
in `Controllers/AuthController.cs`
```cs

```
