# JWT auth
* Update entity
* Generate DTOs


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
