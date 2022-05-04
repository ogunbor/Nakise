using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Enums;
using Domain.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Data.DbContext
{
    public static class DbInitializer
    {
        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            var rolesEnumList = EnumExtension.GetEnumResults<ERole>();
            if (rolesEnumList.Any())
            {
                foreach (var item in rolesEnumList)
                {
                    var roleRecord = context.Roles.Where(x => x.Name.Equals(item.Name));
                    if (roleRecord.FirstOrDefault()?.Name == null)
                    {
                        Role role = new()
                        {
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            Name = item.Name,
                        };
                        await roleManager.CreateAsync(role);
                    }
                }
            }
        }

        public static async Task SeedOrganizationUserData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            if (!context.Organizations.Any())
            {
                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = "Tech4Dev",
                    OrganizationRef = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Organizations.Add(organization);
                await context.SaveChangesAsync();

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "admin@admin.com",
                    Category = "Admin",
                    Status = EUserStatus.Active.ToString(),
                    Verified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    UserName = "admin@admin.com",
                    PhoneNumber = "07036000000",
                    OrganizationId = organization.Id,
                };

                var role = ERole.Admin.ToString();
                user.Password = userManager.PasswordHasher.HashPassword(user, "Admin123@");
                await userManager.CreateAsync(user, "Admin123@");
                if (!(await userManager.IsInRoleAsync(user, role)))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public static async Task SeedActivityData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var activities = new List<Activity>
            {
                new Activity
                {
                    Name = "Call for Application",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.CallForApplication.ToString()
                },
                new Activity
                {
                    Name = "Training",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.Training.ToString()
                },
                new Activity
                {
                    Name = "Survey",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.Survey.ToString()
                },
                new Activity
                {
                    Name = "Assessment",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.Assessment.ToString()
                },
                new Activity
                {
                    Name = "Forms",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.Forms.ToString()
                },
                new Activity
                {
                    Name = "Events",
                    Description = "Brief explanation on this feature will come here, few words",
                    Type = EActivityType.Events.ToString()
                }
            };

            if (!context.Activities.Any())
            {
                await context.AddRangeAsync(activities);
                await context.SaveChangesAsync();
            }
        }
    }
}
