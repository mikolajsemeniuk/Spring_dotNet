using System;
using System.Threading;
using System.Threading.Tasks;
using identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace identity.Data
{
    public class Seed
    {
        public static async Task InitSeed(DataContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            if (await IfNecessaryRolesExist(roleManager))
                return;
            await DeleteMigrateCreateDatabase(context);
            await AddNecessaryRolesAsync(roleManager);
            await AddInitialUsersWithRoles(userManager, roleManager);
        }

        private static async Task<bool> IfNecessaryRolesExist(
            RoleManager<ApplicationRole> roleManager) =>
                await roleManager.RoleExistsAsync("Admin") &&
                await roleManager.RoleExistsAsync("Moderator") &&
                await roleManager.RoleExistsAsync("Member");

        private static async Task DeleteMigrateCreateDatabase(DataContext context)
        {
            var migrator = context.Database.GetService<IMigrator>();

            await context.Database.EnsureDeletedAsync();
            await migrator.MigrateAsync("InitialIdentityCoreDbMigration", new CancellationToken());
            await context.Database.EnsureCreatedAsync();
        }

        private static async Task AddNecessaryRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            await roleManager.CreateAsync(new ApplicationRole { Name = "Moderator" });
            await roleManager.CreateAsync(new ApplicationRole { Name = "Member" });
        }

        private static async Task AddInitialUsersWithRoles(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            var admin = new ApplicationUser
            {
                Email = "admin@mock.com",
                UserName = "admin@mock.com"
            };
            await userManager.CreateAsync(admin, "P@ssw0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

            var moderator = new ApplicationUser
            {
                Email = "moderator@mock.com",
                UserName = "moderator@mock.com"
            };
            await userManager.CreateAsync(moderator, "P@ssw0rd");
            await userManager.AddToRolesAsync(moderator, new[] { "Moderator" });
        }
    }
}