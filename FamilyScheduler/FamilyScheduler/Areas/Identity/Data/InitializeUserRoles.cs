using FamilyScheduler.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyScheduler.Areas.Identity.Data
{
    public class InitializeUsersRoles
    {
        private readonly static string AdministratorRole = "Admin";
        private readonly static string MemberRole = "Member";
        private readonly static string Password = "Testing123$";
        private readonly FamilySchedulerContext _context;

        public InitializeUsersRoles(FamilySchedulerContext context)
        {
            _context = context;
        }

        public async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AuthenticationContext(serviceProvider.GetRequiredService<DbContextOptions<AuthenticationContext>>()))
            {
                var adminID = await EnsureUser(serviceProvider, Password, "admin@chayton.info", "Chayton", "Sutton");
                await EnsureRole(serviceProvider, adminID, AdministratorRole);

                var memberID = await EnsureUser(serviceProvider, Password, "member@chayton.info", "Tyler", "Sutton");
                await EnsureRole(serviceProvider, memberID, MemberRole);
            }
        }

        // Check that user exists with provided email address --> create new user if none exists
        private async Task<string> EnsureUser(IServiceProvider serviceProvider, string userPw, string UserName, string firstName, string lastName)
        {
            // Access the UserManager service
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            if (userManager != null)
            {
                // Find user by email address
                var user = await userManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    // Create new user if none exists
                    user = new ApplicationUser { UserName = UserName };
                    var userAccount = new FamilyScheduler.Models.User();
                    userAccount.UserName = user.UserName;
                    userAccount.FirstName = firstName;
                    userAccount.LastName = lastName;
                    _context.Users.Add(userAccount);
                    await _context.SaveChangesAsync();
                    user.UserAccountID = userAccount.UserID;
                    await userManager.CreateAsync(user, userPw);
                }

                // Confirm the new user so that we can log in
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                return user.Id;
            }
            else
                throw new Exception("userManager null");
        }

        // Check that role exists --> create new rule if none exists
        private async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            // Access RoleManager service
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager != null)
            {
                // Check whether role exists --> if not, create new role with the provided role name
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Retrieve user with the provided ID and add to the specified role
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager != null)
                {
                    var user = await userManager.FindByIdAsync(uid);
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                    throw new Exception("userManager null");

            }
            else
                throw new Exception("roleManager null");
        }
    }
}
