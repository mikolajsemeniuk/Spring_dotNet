# Medium CRUD
* Create Model
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
### Create migration and update db
```sh
dotnet ef database drop
dotnet ef migrations add Todos
dotnet ef database update
```
### Create Input
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
# Create Controller
in `Controllers/TodosController.cs`
```cs

```
