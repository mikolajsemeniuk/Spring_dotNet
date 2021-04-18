# Complex CRUD
* Install AutoMapper
* Create Models
* Configure DataContext
* Create migration
* Add Inputs and Payloads
* Add AutoMapper
* Create Interfaces
* Create Repositories
* Create Controllers
* Configure Services

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
in `DTO/UserInput`
```cs
using System.ComponentModel.DataAnnotations;

namespace test.DTO
{
    public class UserInput
    {
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
```
in `DTO/UserPayload`
```cs
using System.Collections.Generic;

namespace test.DTO
{
    public class UserPayload
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<BookPayload> Books { get; set; }
    }
}
```
in `DTO/BookInput`
```cs
using System.ComponentModel.DataAnnotations;

namespace test.DTO
{
    public class BookInput
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public AuthorInput Author { get; set; }
    }
}
```
in `DTO/BookPayload`
```cs
namespace test.DTO
{
    public class BookPayload
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public AuthorPayload Author { get; set; }
    }
}
```
in `DTO/AuthorInput`
```cs
using System;

namespace test.DTO
{
    public class AuthorInput
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
```
in `DTO/AuthorPayload`
```cs
using System;

namespace test.DTO
{
    public class AuthorPayload
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int BookId { get; set; }
    }
}
```
### Add AutoMapper
in `Helpers/AutoMapperProfiles.cs`
```cs
using AutoMapper;
using test.DTO;
using test.Models;

namespace test.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserInput, User>();
            CreateMap<User, UserPayload>();

            CreateMap<BookInput, Book>()
                .ForMember(destination => destination.Author, opt => opt.MapFrom(source => source.Author));
            CreateMap<Book, BookPayload>()
                .ForMember(destination => destination.Author, opt => opt.MapFrom(source => source.Author));

            CreateMap<AuthorInput, Author>();
            CreateMap<Author, AuthorPayload>();
            
        }
    }
}
```
### Create Interfaces
in `Interfaces/IUserRepository.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using test.DTO;
using test.Models;

namespace test.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserPayload>> GetUsersPayloadAsync();

        Task<UserPayload> GetUserPayloadAsync(int id);
        Task<UserPayload> CreateUserAsync(UserInput input);
        Task<UserPayload> SetUserAsync(int id, UserInput input);
        Task<UserPayload> DeleteUserAsync(int id);

        // ONLY TO DEMONSTRATE
        Task<IEnumerable<User>> GetUsersAsync();
    }
}
```
in `Interfaces/IBookRepository.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using test.DTO;

namespace test.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookPayload>> GetBooksAsync();
        Task<BookPayload> GetBookAsync(int id);
        Task<BookPayload> AddBookAsync(BookInput input);
        Task<BookPayload> SetBookAsync(int id, BookInput input);
        Task<BookPayload> RemoveBookAsync(int id);
    }
}
```
### Create Repositories
in `Services/UserRepository.cs`
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.DTO;
using test.Interfaces;
using test.Models;

namespace test.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserPayload>> GetUsersPayloadAsync() =>
            // await _context.Users
            //     .Include(users => users.Books)
            //     .ThenInclude(books => books.Author)
            //     .Select(user => new UserPayload
            //     {
            //         Id = user.Id,
            //         UserName = user.UserName,
            //         Email = user.Email,
            //         Books = user.Books.Select(book => new BookPayload
            //         {
            //             Id = book.Id,
            //             Title = book.Title,
            //             Description = book.Description,
            //             UserId = book.UserId,
            //             Author = new AuthorPayload
            //             {
            //                 Id = book.Author.Id,
            //                 Name = book.Author.Name,
            //                 FullName = book.Author.FullName,
            //                 DateOfBirth = book.Author.DateOfBirth,
            //                 BookId = book.Author.BookId
            //             }
            //         }).ToList()
            //     })
            //     .ToListAsync();
            await _context.Users
                .ProjectTo<UserPayload>(_mapper.ConfigurationProvider)
                .AsSingleQuery()
                .ToListAsync();
            

        public async Task<UserPayload> GetUserPayloadAsync(int id) =>
            // await _context.Users
            //     .Where(user => user.Id == id)
            //     .Include(user => user.Books)
            //     .ThenInclude(book => book.Author)
            //     .Select(user => new UserPayload
            //     {
            //         Id = user.Id,
            //         UserName = user.UserName,
            //         Email = user.Email,
            //         Books = user.Books.Select(book => new BookPayload
            //         {
            //             Id = book.Id,
            //             Title = book.Title,
            //             Description = book.Description,
            //             UserId = book.UserId,
            //             Author = new AuthorPayload
            //             {
            //                 Id = book.Author.Id,
            //                 Name = book.Author.Name,
            //                 FullName = book.Author.FullName,
            //                 DateOfBirth = book.Author.DateOfBirth,
            //                 BookId = book.Author.BookId
            //             }
            //         }).ToList()
            //     })    
            //     .SingleAsync();
            await _context.Users
                .Where(category => category.Id == id)
                .ProjectTo<UserPayload>(_mapper.ConfigurationProvider)
                .SingleAsync();
        
        public async Task<UserPayload> CreateUserAsync(UserInput input)
        {
            // var user = new User
            // {
            //     UserName = input.UserName,
            //     Email = input.Email
            // };
            var user = new User();
            _mapper.Map(input, user);

            await _context.Users.AddAsync(user);
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");

            // return new UserPayload
            // {
            //     Id = user.Id,
            //     UserName = user.UserName,
            //     Email = user.Email
            // };
            var userPayload = new UserPayload();
            _mapper.Map(user, userPayload);

            return userPayload;
        }

        public async Task<UserPayload> SetUserAsync(int id, UserInput input)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("there is no user with id: " + id);

            // user.UserName = input.UserName;
            // user.Email = input.Email;
            _mapper.Map(input, user);

            _context.Users.Update(user);
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");

            // return new UserPayload
            // {
            //     Id = user.Id,
            //     UserName = user.UserName,
            //     Email = user.Email
            // };
            var userPayload = new UserPayload();
            _mapper.Map(user, userPayload);
            return userPayload;
        }

        public async Task<UserPayload> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("there is no user with id: " + id);

            _context.Users.Remove(user);
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");

            // return new UserPayload
            // {
            //     Id = user.Id,
            //     UserName = user.UserName,
            //     Email = user.Email
            // };
            var userPayload = new UserPayload();
            _mapper.Map(user, userPayload);
            return userPayload;
        }

        // ONLY TO DEMONSTRATE
        public async Task<IEnumerable<User>> GetUsersAsync() =>
            await _context.Users.Include(user => user.Books).ToListAsync();
    }
}
```
in `Services/BookRepository.cs`
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.DTO;
using test.Interfaces;
using test.Models;

namespace test.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BookRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookPayload>> GetBooksAsync() =>
            await _context.Books
                // .Include(book => book.Author)
                // .Select(book => new BookPayload
                // {
                //     Id = book.Id,
                //     Title = book.Title,
                //     Description = book.Description,
                //     UserId = book.UserId, // should be `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
                //     Author = new AuthorPayload
                //     {
                //         Id = book.Author.Id,
                //         Name = book.Author.Name,
                //         FullName = book.Author.FullName,
                //         DateOfBirth = book.Author.DateOfBirth,
                //         BookId = book.Author.BookId
                //     }
                // })
                .ProjectTo<BookPayload>(_mapper.ConfigurationProvider)
                .AsSingleQuery()
                .ToListAsync();

        public async Task<BookPayload> GetBookAsync(int id) =>
            await _context.Books
                //     .Where(book => book.Id == id)
                //     .Include(book => book.Author)
                //     .Select(book => new BookPayload
                //     {
                //         Id = book.Id,
                //         Title = book.Title,
                //         Description = book.Description,
                //         UserId = book.UserId, // should be `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
                //         Author = new AuthorPayload
                //         {
                //             Id = book.Author.Id,
                //             Name = book.Author.Name,
                //             FullName = book.Author.FullName,
                //             DateOfBirth = book.Author.DateOfBirth,
                //             BookId = book.Author.BookId
                //         }
                //     })
                //     .SingleAsync();
                .Where(book => book.Id == id)
                .ProjectTo<BookPayload>(_mapper.ConfigurationProvider)
                .AsSingleQuery()
                .SingleAsync();

        public async Task<BookPayload> AddBookAsync(BookInput input)
        {
            // var book = new Book()
            // {
            //     Title = input.Title,
            //     Description = input.Description,
            //     UserId = input.UserId, // should be `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
            //     Author = new Author
            //     {
            //         Name = input.Author.Name,
            //         FullName = input.Author.FullName,
            //         DateOfBirth = input.Author.DateOfBirth
            //     }
            // };
            var book = new Book();
            _mapper.Map(input, book);

            await _context.Books.AddAsync(book);
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");

            // var bookPayload = new BookPayload()
            // {
            //     Id = book.Id,
            //     Title = book.Title,
            //     Description = book.Description,
            //     Author = new AuthorPayload
            //     {
            //         Id = book.Author.Id,
            //         Name = book.Author.Name,
            //         FullName = book.Author.FullName,
            //         DateOfBirth = book.Author.DateOfBirth,
            //         BookId = book.Author.BookId
            //     }
            // };
            var bookPayload = new BookPayload();
            _mapper.Map(book, bookPayload);

            return bookPayload;
        }

        public async Task<BookPayload> SetBookAsync(int id, BookInput input)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new Exception("Book with id " + id + " not found");

            // book.Title = input.Title;
            // book.Description = input.Description;
            // book.UserId = input.UserId; // should be `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
            _mapper.Map(input, book);
            
            var author = await _context.Authors.SingleAsync(author => author.BookId == id);
            // author.Name = input.Author.Name;
            // author.FullName = input.Author.FullName;
            // author.DateOfBirth = input.Author.DateOfBirth;
            _mapper.Map(input.Author, author);

            _context.Books.Update(book);
            _context.Authors.Update(author);
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");
            
            // return new BookPayload
            // {
            //     Id = book.Id,
            //     Title = book.Title,
            //     Description = book.Description,
            //     UserId = book.UserId,
            //     Author = new AuthorPayload
            //     {
            //         Id = book.Author.Id,
            //         Name = book.Author.Name,
            //         FullName = book.Author.FullName,
            //         DateOfBirth = book.Author.DateOfBirth,
            //         BookId = book.Author.BookId
            //     }
            // };
            var bookPayload = new BookPayload();
            _mapper.Map(book, bookPayload);
            return bookPayload;
        }

        public async Task<BookPayload> RemoveBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            var author = await _context.Authors.SingleAsync(author => author.BookId == id);
            _context.Books.Remove(book);

            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");

            // return new BookPayload
            // {
            //     Id = book.Id,
            //     Title = book.Title,
            //     Description = book.Description,
            //     UserId = book.UserId, // should be `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
            //     Author = new AuthorPayload
            //     {
            //         Id = author.Id,
            //         Name = author.Name,
            //         FullName = author.FullName,
            //         DateOfBirth = author.DateOfBirth,
            //         BookId = author.BookId
            //     }
            // };
            var bookPayload = new BookPayload();
            _mapper.Map(book, bookPayload);
            return bookPayload;
        }
    }
}
```
### Create Controllers
in `Controllers/UserController.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using test.DTO;
using test.Interfaces;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPayload>>> GetUsersPayloadAsync() =>
            Ok(await _repository.GetUsersPayloadAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UserPayload>>> GetUserPayloadAsync(int id) =>
            Ok(await _repository.GetUserPayloadAsync(id));

        [HttpPost]
        public async Task<ActionResult<UserPayload>> CreateUser([FromBody] UserInput input) =>
            Ok(await _repository.CreateUserAsync(input));
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserPayload>> DeleteUser(int id) =>
            Ok(await _repository.DeleteUserAsync(id));


        // ONLY TO DEMONSTRATE
        [HttpGet("demonstrate")]
        public async Task<ActionResult<IEnumerable<UserPayload>>> GetUsersAsync()
        {
            var users = await _repository.GetUsersAsync();
            var usersToReturn = _mapper.Map<IEnumerable<UserPayload>>(users);
            return Ok(usersToReturn);
        }
    }
}
```
in `Controllers/BookController.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test.DTO;
using test.Interfaces;
using test.Models;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BookController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks() =>
            Ok(await _repository.GetBooksAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook(int id) =>
            Ok(await _repository.GetBookAsync(id));

        [HttpPost]
        public async Task<ActionResult<BookPayload>> AddBook([FromBody] BookInput input) =>
            Ok(await _repository.AddBookAsync(input));

        [HttpPut("{id}")]
        public async Task<ActionResult<BookPayload>> SetBook([FromRoute] int id, [FromBody] BookInput input) =>
            Ok(await _repository.SetBookAsync(id, input));

        [HttpDelete("{id}")]
        public async Task<ActionResult<BookPayload>> RemoveBook([FromRoute] int id) =>
            Ok(await _repository.RemoveBookAsync(id));
    }
}
```
### Configure Services
in `Startup.cs`
```cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using test.Data;
using test.Helpers;
using test.Interfaces;
using test.Middleware;
using test.Services;

namespace test
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "test", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            //     app.UseSwagger();
            //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "test v1"));
            // }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors(options => options
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://localhost:4200"));

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
