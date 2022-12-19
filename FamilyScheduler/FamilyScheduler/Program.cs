using FamilyScheduler.Areas.Identity.Data;
using FamilyScheduler.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FamilySchedulerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));

// Add common Identity services (login, registration, etc.)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add role-related services
    .AddEntityFrameworkStores<AuthenticationContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add authorization service and the admin user policy
builder.Services.AddAuthorization(options =>
{
    // Add global authentication requirement for the app
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
         .Build();

    // Define a policy that requires user to be in a specific role 
    // -- used for View-based authorization
    options.AddPolicy("RequireAdministratorRole", policy =>
        policy.RequireRole("Admin,SuperUser"));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FamilySchedulerContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured creating the DB.");
    }

    try
    {
        var context = services.GetRequiredService<AuthenticationContext>();
        context.Database.Migrate();
        var accounts = new InitializeUsersRoles(services.GetRequiredService<FamilySchedulerContext>());
        accounts.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        // Something went wrong
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the users and roles.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
