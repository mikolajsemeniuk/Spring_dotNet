# Complex CRUD
* Create Models
* Configure DataContext

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
