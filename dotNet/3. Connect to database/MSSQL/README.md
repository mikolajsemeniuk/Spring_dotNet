# Connect to MSSQL
1. Create first entity
1. Install `Microsoft.EntityFrameworkCore.SqlServer`
1. Install `Microsoft.EntityFrameworkCore.Design`
1. Install `dotnet-ef` **(if you do not have it)**
1. Create Class derives from DbContext
1. Configure service
1. Add ConnectionString
1. Update database

### Create first Entity
Create first Entity which would be represent you table in database
* **You do not have to specify ID if** `id` column would be named as `id`, `ID` or `Id` then asp.net.core will figure it out



Example code below:
```cs
namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }

    }
}
```
### Install `Microsoft.EntityFrameworkCore.SqlServer`
you should be able to install this package via nuget from vs/vscode or nuget gallery in vscode by pressing `cmd + shift + p` and then `NuGet Package Manager: Add Package`.
After completed instalation your API.csproj should look like below:
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
  </ItemGroup>
</Project>
```
### Install `Microsoft.EntityFrameworkCore.Design`
you should be able to install this package via nuget from vs/vscode or nuget gallery in vscode by pressing `cmd + shift + p` and then `NuGet Package Manager: Add Package`.
After completed instalation your API.csproj should look like below:
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
  </ItemGroup>
</Project>
```
### Install `dotnet-ef` **(if you do not have it)**
Install `dotnet-ef` by typing in your terminal
```sh
dotnet tool install --global dotnet-ef --version 5.0.0
```
more info about this package and version you could find here [www.nuget.org](https://www.nuget.org/packages/dotnet-ef/5.0.0) 
### Create Class derives from DbContext
Next we have to create `DataContext` class which would be injected via services in `startup.cs`


example code:
```cs
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // below should be tables you would like dotnet to create
        public DbSet<AppUser> Users { get; set; }
        // public DbSet<AppUser> SecondTable { get; set; }
        // public DbSet<AppUser> ThirdTable { get; set; }
        // etc...
    }
}
```
### Configure service
Let's configure service to be injected in `Startup.cs`
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
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

namespace API
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
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
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
### Add ConnectionString
Let's create configure connection string in `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=asp;User Id=sa;Password=super_secret_password"
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
### Update database
Let's create first migration add her name and specify folder for output and then update our db
```sh
dotnet ef migrations add InitialCreate -o Data/Migrations # To undo this action, use 'ef migrations remove'
dotnet ef database update        
```
## Then you should see created table in your database, congratulations ;)
