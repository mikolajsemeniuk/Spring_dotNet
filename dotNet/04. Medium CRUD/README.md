# Medium CRUD
* Create Model
* Create migration and update db

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
# Create Controller
in `Controllers/TodosController.cs`
```cs

```
