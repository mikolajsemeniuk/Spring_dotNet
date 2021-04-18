# Medium CRUD
* Create Model
* Modify DbContext
* Create migration and update db
* Create Input and Payload
* Create Interface
* Create Repository
* Register service
* Create Controller

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
using System.Collections.Generic;
using System.Threading.Tasks;
using test.DTO;

namespace test.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoPayload>> GetTodosAsync();
        Task<TodoPayload> GetTodoAsync(int id);
        Task<TodoPayload> AddTodoAsync(TodoInput input);
        Task<TodoPayload> SetTodoAsync(int id, TodoInput input);
        Task<TodoPayload> RemoveTodoAsync(int id);
        Task CheckIfAllSavedAsync(); 
    }
}
```
### Create Repository
in `Services/TodoRepository.cs`
```cs
using System;
using System.Collections.Generic;
using System.Linq;
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

        public TodoRepository(DataContext context) =>
            _context = context;
            
        public async Task<IEnumerable<TodoPayload>> GetTodosAsync() =>
            await _context.Todos
                .Select(todo => new TodoPayload { Id = todo.Id, Title = todo.Title, Description = todo.Description, Created = todo.Created, Updated = todo.Updated, IsDone = todo.IsDone })
                .ToListAsync();

        public async Task<TodoPayload> GetTodoAsync(int id) =>
            await _context.Todos
                .Where(todo => todo.Id == id)
                .Select(todo => new TodoPayload { Id = todo.Id, Title = todo.Title, Description = todo.Description, Created = todo.Created, Updated = todo.Updated, IsDone = todo.IsDone })
                .SingleAsync();

        public async Task<TodoPayload> AddTodoAsync(TodoInput input)
        {
            var todo = new Todo { Title = input.Title, Description = input.Description, IsDone = input.IsDone };
            await _context.Todos.AddAsync(todo);
            await CheckIfAllSavedAsync();
            return new TodoPayload 
                { Id = todo.Id, Title = todo.Title, Description = todo.Description, Created = todo.Created, Updated = todo.Updated, IsDone = todo.IsDone };
        }

        public async Task<TodoPayload> SetTodoAsync(int id, TodoInput input)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                throw new Exception("Could not find an item with id: " + id);
            todo.Title = input.Title;
            todo.Description = input.Description;
            todo.IsDone = input.IsDone;
            todo.Updated = DateTime.Now;
            _context.Todos.Update(todo);
            await CheckIfAllSavedAsync();
            return new TodoPayload
                { Id = todo.Id, Title = todo.Title, Description = todo.Description, Created = todo.Created, Updated = todo.Updated, IsDone = todo.IsDone };
        }

        public async Task<TodoPayload> RemoveTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
                throw new Exception("Could not find an item with id: " + id);
            _context.Todos.Remove(todo);
            await CheckIfAllSavedAsync();
            return new TodoPayload
                { Id = todo.Id, Title = todo.Title, Description = todo.Description, Created = todo.Created, Updated = todo.Updated, IsDone = todo.IsDone };
        }

        public async Task CheckIfAllSavedAsync()
        {
            if (!(await _context.SaveChangesAsync() > 0))
                throw new Exception("something gone wrong");
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
in `Controllers/TodoController.cs`
```cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test.DTO;
using test.Interfaces;

namespace test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private ITodoRepository _repository;

        public TodoController(ITodoRepository repository) =>
            _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoPayload>>> GetTodos() =>
            Ok(await _repository.GetTodosAsync());

        [HttpPost]
        public async Task<ActionResult<TodoPayload>> AddTodo([FromBody] TodoInput input) =>
            Ok(await _repository.AddTodoAsync(input));

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoPayload>> GetTodo([FromRoute] int id) =>
            Ok(await _repository.GetTodoAsync(id));

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoPayload>> SetTodo([FromRoute] int id, [FromBody] TodoInput input) =>
            Ok(await _repository.SetTodoAsync(id, input));

        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoPayload>> DeleteTodo([FromRoute] int id) =>
            Ok(await _repository.RemoveTodoAsync(id));
    }
}
```
