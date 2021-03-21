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


        
        // Relation belov is demanded by
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
        // Relation belov is demanded by
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
        // Relation belov is demanded by
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
