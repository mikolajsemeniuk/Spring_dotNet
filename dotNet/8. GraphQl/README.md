# GraphQl
* Install packages
* Create model
* Create interface
* Create service
* Create Types
* Create Query
* Create mutation
* Create schema
* Configure services
* Test
* Db connection

### Install packages
```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.0" NoWarn="NU1605"/>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.0" NoWarn="NU1605"/>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.6.3"/>
    
    <!-- Install packages below -->
    <PackageReference Include="GraphQL.Server.Transports.AspNetCore.SystemTextJson" Version="5.0.0"/>
    <PackageReference Include="graphiql" Version="2.0.0"/>
  </ItemGroup>
</Project>
```
### Create model
in `Models/Product.cs`
```cs
namespace graph.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
```
### Create interface
in `Interfaces/IProductService.cs`
```cs
using System.Collections.Generic;
using graph.Models;

namespace graph.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        Product AddProduct(Product product);
        Product UpdateProduct(int id, Product product);
        void DeleteProduct(int id);
    }
}
```
### Create service
in `Services/ProductService.cs`
```cs
using System.Collections.Generic;
using graph.Interfaces;
using graph.Models;

namespace graph.Services
{
    public class ProductService : IProductService
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 0, Name = "Coffe", Price = 15},
            new Product { Id = 1, Name = "Tea", Price = 25}
        }; 
        public Product AddProduct(Product product)
        {
            products.Add(product);
            return product;
        }

        public void DeleteProduct(int id)
        {
            products.RemoveAt(id);
        }

        public List<Product> GetAllProducts()
        {
            return products;
        }

        public Product GetProductById(int id)
        {
            return products.Find(product => product.Id == id);
        }

        public Product UpdateProduct(int id, Product product)
        {
            products[id] = product;
            return product;
        }
    }
}
```
### Create Types
in `Types/ProductType.cs`
```cs
using graph.Models;
using GraphQL.Types;

namespace graph.Type
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(p => p.Id);
            Field(p => p.Name);
            Field(p => p.Price);
        }
    }
}
```
in `Types/ProductInputType.cs`
```cs
using GraphQL.Types;

namespace graph.Type
{
    public class ProductInputType : InputObjectGraphType
    {
        public ProductInputType()
        {
            Field<IntGraphType>("id");
            Field<StringGraphType>("name");
            Field<FloatGraphType>("price");
        }
    }
}
```
### Create Query
in `Queries/ProductQuery.cs`
```cs
using graph.Interfaces;
using graph.Type;
using GraphQL;
using GraphQL.Types;

namespace graph.Query
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IProductService productService)
        {
            Field<ListGraphType<ProductType>>(
                "products",
                resolve: context => 
                { 
                    return productService.GetAllProducts(); 
                }
            );
            Field<ProductType>(
                "product", 
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context => { 
                    return productService.GetProductById(context.GetArgument<int>("id")); 
                }
            );
        }
    }
}
```
### Create mutation
in `Mutations/ProductMutation.cs`
```cs
using graph.Interfaces;
using graph.Models;
using graph.Type;
using GraphQL;
using GraphQL.Types;

namespace graph.Mutation
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IProductService productService)
        {
            Field<ProductType>(
                "createProduct",
                arguments: 
                    new QueryArguments(new QueryArgument<ProductInputType> { Name = "product" }),
                resolve: 
                    context => 
                        {
                            return productService.AddProduct(context.GetArgument<Product>("product"));
                        }
            );

            Field<ProductType>(
                "updateProduct",
                arguments: 
                    new QueryArguments(
                        new QueryArgument<IntGraphType> { Name = "id" },
                        new QueryArgument<ProductInputType> { Name = "product" }
                    ),
                resolve:
                    context =>
                        {
                            var productId = context.GetArgument<int>("id");
                            var productObj = context.GetArgument<Product>("product");
                            return productService.UpdateProduct(productId, productObj);
                        }
            );
            Field<StringGraphType>(
                "deleteProduct",
                arguments:
                    new QueryArguments(
                        new QueryArgument<IntGraphType> { Name = "id" }
                    ),
                resolve:
                    context =>
                    {
                        var productId = context.GetArgument<int>("id");
                        productService.DeleteProduct(productId);
                        return $"The product against the {productId} was deleted";
                    }
            );
        }
    }
}
```
### Create schema
in `Schemas/ProductSchema.cs`
```cs
using graph.Mutation;
using graph.Query;

namespace graph.Schema
{
    public class ProductSchema : GraphQL.Types.Schema
    {
        public ProductSchema(ProductQuery productQuery, ProductMutation productMutation)
        {
            Query = productQuery;
            Mutation = productMutation;
        }
    }
}
```
### Configure services
in `Startup.cs`
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graph.Interfaces;
using graph.Mutation;
using graph.Query;
using graph.Schema;
using graph.Services;
using graph.Type;
using GraphiQl;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace graph
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
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ProductType>();
            services.AddScoped<ProductQuery>();
            services.AddScoped<ProductMutation>();
            services.AddScoped<ISchema, ProductSchema>();
            services.AddGraphQL(options => {
                options.EnableMetrics = false;
            }).AddSystemTextJson();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "graph", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "graph v1"));
            }

            // app.UseHttpsRedirection();

            // app.UseRouting();

            // app.UseAuthorization();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });
            app.UseGraphiQl("/graph");
            app.UseGraphQL<ISchema>();
        }
    }
}
```
### Test
go to `https://localhost:5001/graph/`
```graphql
query GetAll {
  products {
    id,
    name,
    price
  }
}
query GetSingleById {
  product(id: 1) {
    id,
    name,
    price
  }
}
mutation AddProduct($product: ProductInputType) {
  createProduct(product: $product) {
    id,
    name,
    price
  }
}
# Query Variable
{
  "product": {
    "id": 2,
    "name": "passionfruit",
    "price": 35
  }
}
mutation UpdateProduct($id: Int, $product: ProductInputType) {
  updateProduct(id: $id product: $product) {
    id,
    name,
    price
  }
}
# Query Variable
{
  "id": 2,
  "product": {
    "id": 2,
    "name": "pineapple",
    "price": 40
  }
}
mutation DeleteProduct($id:Int) {
	deleteProduct(id:$id)
}
# Query Variable
{
  "id": 2
}
```
### Db connection
Add Db connection in section 3 and

modify `Interfaces/IProductService.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using graphq.Models;

namespace graphq.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(int id, Product product);
        Task DeleteProduct(int id);
    }
}
```
and modify `Services/ProductService.cs` to have fully functional CRUD
```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using graphq.Data;
using graphq.Interfaces;
using graphq.Models;
using Microsoft.EntityFrameworkCore;

namespace graphq.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<Product> UpdateProduct(int id, Product product)
        {
            var obj = await _context.Products.FindAsync(id);
            obj.Name = product.Name;
            obj.Price = product.Price;
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task DeleteProduct(int id)
        {
            var obj = await _context.Products.FindAsync(id);
            _context.Products.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
```
