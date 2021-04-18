# Medium CRUD
* Create Model
* Modify DbContext
* Create migration and update db
* Create Input
* Create Interface

### Create Model
in `Models/Todo.cs`
```cs
using System;

namespace test.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Boolean IsDone { get; set; }
    }
}
```
### Modify DbContext
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
        public DbSet<Todo> Todos { get; set; }
    }
}
```
### Create migration and update db
```sh
dotnet ef database drop
dotnet ef migrations add Todos
dotnet ef database update
```
### Create Input and Payload
in `DTO/TodoInput.cs`
```cs
using System;
using System.ComponentModel.DataAnnotations;

namespace test.DTO
{
    public class TodoInput
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Boolean IsDone { get; set; }
    }
}
```
in `DTO/TodoPayload.cs`
```cs
using System;

namespace test.DTO
{
    public class TodoPayload
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Boolean IsDone { get; set; }
    }
}
```
### Create Interface
in `Interfaces/ITodoRepository.cs`
```cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using test.DTO;
using test.Models;

namespace test.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetTodosAsync();
        Task<Todo> GetTodoAsync(int id);
        Task<Todo> AddTodoAsync(TodoInput input);
        Task<Todo> SetTodoAsync(int id, TodoInput input);
        Task<Todo> DeleteTodoAsync(int id);
        Task<Boolean> SaveAllAsync();
    }
}
```
### Create Repository
in `Services/TodoRepository.cs`
```cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.DTO;
using test.Interfaces;
using test.Models;

namespace test.Services
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DataContext _context;

        public TodoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Todo>> GetTodosAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo> GetTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                throw new Exception("Could not find an item with this id");
            return todo;
        }

        public async Task<Todo> AddTodoAsync(TodoInput input)
        {
            var todo = new Todo 
            {
                Title = input.Title,
                Description = input.Description,
                IsDone = input.IsDone
            };
            await _context.Todos.AddAsync(todo);
            return todo;
        }

        public async Task<Todo> SetTodoAsync(int id, TodoInput input)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                throw new Exception("Could not find an item with this id");
            todo.Title = input.Title;
            todo.Description = input.Description;
            todo.IsDone = input.IsDone;
            todo.Updated = DateTime.Now;
            _context.Todos.Update(todo);
            return todo;
        }

        public async Task<Todo> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                throw new Exception("Could not find an item with this id");
            _context.Todos.Remove(todo);
            return todo;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
```
### Register service
in `Startup.cs`
```cs
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddScoped<ITodoRepository, TodoRepository>();
    // ...
}
```
### Create Controller
in `Controllers/TodosController.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test.Data;
using test.DTO;
using test.Interfaces;
using test.Models;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly DataContext _context;
        private ITodoRepository _repository;

        public TodosController(DataContext context, ITodoRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            return Ok(await _repository.GetTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo([FromRoute] int id)
        {
            return Ok(await _repository.GetTodoAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodo([FromBody] TodoInput input)
        {
            var todo = await _repository.AddTodoAsync(input);
            if (!await _repository.SaveAllAsync())
                return BadRequest("something gone wrong");
            return Ok(todo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> SetTodo([FromRoute] int id, [FromBody] TodoInput input)
        {
            var todo = await _repository.SetTodoAsync(id, input);
            if (!await _repository.SaveAllAsync())
                return BadRequest("something gone wrong");
            return Ok(todo);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Todo>> DeleteTodo([FromRoute] int id)
        {
            var todo = await _repository.DeleteTodoAsync(id);
            if (!await _repository.SaveAllAsync())
                return BadRequest("something gone wrong");
            return Ok(todo);
        }
    }
}
```
