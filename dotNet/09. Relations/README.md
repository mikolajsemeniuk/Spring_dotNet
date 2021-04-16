# Relations
* [one to many](https://www.learnentityframeworkcore.com/configuration/one-to-many-relationship-configuration)
* [one to one](https://www.learnentityframeworkcore.com/configuration/one-to-one-relationship-configuration)
* [many to many](https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration)
* Likes example implementation

### Likes example implementation
in `Models/User.cs`
```cs
using System.Collections.Generic;

namespace test.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set }
        public ICollection<UserLike> LikedUsers { get; set; }
    }
}
```
in `Models/UserLike.cs`
```cs
namespace test.Models
{
    public class UserLike
    {
        public int SourceUserId { get; set }
        public User SourceUser { get; set; }
        
        public int LikedUserId { get; set; }
        public User LikedUser { get; set; }
    }
}
```
in `Data/DataContext.cs`
```cs
using Microsoft.EntityFrameworkCore;

namespace test.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });
                
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                // Use DeleteBehavior.NoAction
                // If you are using SQL Server
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                // Use DeleteBehavior.NoAction
                // If you are using SQL Server
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```
Create migration and update database
```sh
dotnet ef database drop
dotnet ef migrations add UsersLikes
dotnet ef database update
```
