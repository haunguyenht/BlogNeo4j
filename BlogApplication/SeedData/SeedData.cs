using BlogApplication.Constant;
using BlogApplication.Models;
using BlogApplication.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.SeedData
{
    public static class SeedData
    {
        

        public static async Task InitializeAsync(IServiceProvider services, IRepository _repository)
        {
            var roleManager = services
           .GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRolesAsync(roleManager);

            var userManager = services
            .GetRequiredService<UserManager<ApplicationUser>>();
            await EnsureTestAdminAsync(userManager, _repository);
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var alreadyExists = await roleManager
            .RoleExistsAsync(Constants.AdministratorRole);

            if (alreadyExists) return;

            await roleManager.CreateAsync(
            new IdentityRole(Constants.AdministratorRole));
        }

        private static async Task EnsureTestAdminAsync(UserManager<ApplicationUser> userManager, IRepository _repository)
        {
            var testAdmin = await userManager.Users
            .Where(x => x.UserName == "admin@todo.local")
            .SingleOrDefaultAsync();

            if (testAdmin != null) return;

            testAdmin = new ApplicationUser
            {
                UserName = "admin@todo.local",
                Email = "admin@todo.local"
            };
            await userManager.CreateAsync(testAdmin, "123456789");
            await userManager.AddToRoleAsync(testAdmin, Constants.AdministratorRole);
            await _repository.CreateUser(testAdmin);

        }
    }
}
