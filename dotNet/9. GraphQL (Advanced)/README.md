# GraphQL (Advanced)
* Connect to db
* Install packages
* Simple Query
  - Create Model and DbContext
  - Add First Query
  - Configure services
  - Test
* Parallel Query
  - Configure services
  - Modify Query
  - Test
* Using projection
  - Add Second Model and modify DbContext
  - Modify Query
  - Modify Services
  - Test
* Using GraphsTypes (to get single command belongs to platform)

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
### Simple Query
> Create Model and DbContext

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
> Add First Query

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
> Configure services

in `Startup.cs`
```cs
using les.Data;
using les.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace les
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            // ADD THIS
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "les", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "les v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // MODIFY THIS
                // endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
```
> Test
```graphql
# Basic platform query
query {
  platform {
    id,
    name
  }
}
```
### Parallel Query
> Configure Services

in `Startup.cs`
```cs
using les.Data;
using les.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace les
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPooledDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            // ADD THIS
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "les", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "les v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // MODIFY THIS
                // endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
```
> Modify Query

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
        [UseDbContext(typeof(AppDbContext))]
        public IQueryable<Platform> Platform([ScopedService] AppDbContext context)
        {
            return context.Platforms;
        }
    }
}
```
> Test
```graphql
# Parallel query using PolledDbContext 
query {
  a: platform {
    id,
    name
  },
  b: platform {
    id,
    name
  },
  c: platform {
    id,
    name
  }
}
```
### Using projection
> Add Second Model and modify DbContext

in `Models/Platform.cs`
```cs
using System.Collections.Generic;
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
        public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}
```
in `Models/Command.cs`
```cs
using System.ComponentModel.DataAnnotations;

namespace les.Models
{
    public class Command
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string HowTo { get; set;}
        [Required]
        public string CommandLine { get; set;}
        [Required]
        public int PlatformId { get; set;}
        public Platform Platform { get; set;}
    }
}
```
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
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(p => p.Platform)
                .HasForeignKey(p => p.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(p => p.PlatformId);
        }
    }
}
```
> Modify Query

in `GraphQL/Query.cs`
```cs
using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using les.Data;
using les.Models;

namespace les.GraphQL
{
    public class Query
    {
        // Name of function will appear as endpoint in GraphQL
        // and as a option in query 
        [UseDbContext(typeof(AppDbContext))]
        [UseProjection]
        public IQueryable<Platform> Platform([ScopedService] AppDbContext context)
        {
            return context.Platforms;
        }

        // Name of function will appear as endpoint in GraphQL
        // and as a option in query 
        [UseDbContext(typeof(AppDbContext))]
        [UseProjection]
        public IQueryable<Command> Command([ScopedService] AppDbContext context)
        {
            return context.Commands;
        }
    }
}
```
> Modify Services

in `Startup.cs`
```cs
using les.Data;
using les.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace les
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // MODIFY THIS
            services.AddPooledDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                // ADD THIS
                .AddProjections();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "les", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "les v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
```
> Test
```graphql
# Using projection
query {
  platform {
    id,
    name,
    commands {
      id,
      howTo,
      commandLine
    }
  }
}
```

```
# using GraphsTypes
query {
  command {
    id,
    howTo,
    commandLine,
    platformId,
    platform {
      name
    }
  }
}
```
### Using GraphsTypes (to get single command belongs to platform)
