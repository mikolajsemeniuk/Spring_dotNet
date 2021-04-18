# Complex CRUD
* Install AutoMapper
* Create Models
* Configure DataContext
* Create migration
* Add Inputs and Payloads
* Add AutoMapper

### Install AutoMapper
Install AutoMapper
```sh
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection -v 8.1.1
```
### Create Models
in `Models/User.cs`
```cs
using System.Collections.Generic;

namespace test.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
```
in `Models/Book.cs`
```cs
using System.Collections.Generic;

namespace test.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Category> Categories { get; set; }
        public Author Author { get; set; }
    }
}
```
in `Models/Category.cs`
```cs
using System.Collections.Generic;

namespace test.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
```
in `Models/Author.cs`
```cs
using System;

namespace test.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
```
### Configure DataContext
in `Data/DataContext.cs`
```cs
using Microsoft.EntityFrameworkCore;
using test.Models;

namespace test.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Specify relations here
        }
    }
}
```
### Create migration
```sh
dotnet ef database drop
dotnet ef migrations add Books
dotnet ef database update
```
### Add Inputs and Payloads
```cs

```

### Add AutoMapper
in `Helpers/AutoMapperProfiles.cs`
```cs

```
