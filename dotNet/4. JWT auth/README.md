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
