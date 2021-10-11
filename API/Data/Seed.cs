using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManeger,
            RoleManager<AppRole> roleManager)
        {
            if (await userManeger.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>{
                new AppRole {Name= "Member"},
                new AppRole {Name= "Admin"},
                new AppRole {Name= "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }


            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();

                await userManeger.CreateAsync(user, "Pa$$w0rd");

                await userManeger.AddToRoleAsync(user, "Member");

            }
            var admin = new AppUser{
                UserName = "admin"
            };

            await userManeger.CreateAsync(admin,"Pa$$w0rd");
            await userManeger.AddToRolesAsync(admin,new []{"Admin","Moderator"});

        }
    }
}