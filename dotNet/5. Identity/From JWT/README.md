# Identity (From JWT)
* Add package
* Update entities

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
