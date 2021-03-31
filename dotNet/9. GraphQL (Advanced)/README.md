# GraphQL (Advanced)
* Connect to db
* Install packages
* Create Model and DbContext

### Connect to db
Connect to db from section 3
### Install packages
Install packages `HotChocolate.AspNetCore` and `HotChocolate.Data.EntityFramework` and `GraphQL.Server.Ui.Voyager`
```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GraphQL.Server.Ui.Voyager" Version="5.0.0" />
    <PackageReference Include="HotChocolate.AspNetCore" Version="11.1.0" />
    <PackageReference Include="HotChocolate.Data.EntityFramework" Version="11.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.6.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.0" />
  </ItemGroup>
</Project>
```
### Create Model and DbContext
in `Data/AppDbContext.cs`
```cs
using les.Models;
using Microsoft.EntityFrameworkCore;

namespace les.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);

            builder
                .Entity<Platform>()
                .HasData(
                    new Platform 
                    { 
                        Id = 1,
                        Name = ".NET",
                        LicenseKey = "7820707"
                    },
                    new Platform 
                    { 
                        Id = 2,
                        Name = "Docker",
                        LicenseKey = "988788"
                    },
                    new Platform 
                    { 
                        Id = 3,
                        Name = "Windows",
                        LicenseKey = "2327312"
                    },
                    new Platform 
                    { 
                        Id = 4,
                        Name = "test",
                        LicenseKey = "2323194"
                    }
                );
        }
    }
}
```
in `Models/Platform.cs`
```cs
using System.ComponentModel.DataAnnotations;

namespace les.Models
{
    public class Platform
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string LicenseKey { get; set; }
    }
}
```
### Add First Query
in `GraphQL/Query.cs`
```cs
using System.Linq;
using HotChocolate;
using les.Data;
using les.Models;

namespace les.GraphQL
{
    public class Query
    {
        // Name of function will appear as endpoint in GraphQL
        // and as a option in query 
        public IQueryable<Platform> Platform([Service] AppDbContext context)
        {
            return context.Platforms;
        }
    }
}
```
