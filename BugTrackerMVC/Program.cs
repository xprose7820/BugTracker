using BugTracker.Data;
using BugTracker.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BugTracker.Service.Login;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Microsoft.SqlServer.Management.Smo;
using BugTracker.Service.User;
using BugTracker.Service.Project;
using BugTracker.Service.Ticket;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
           builder.Configuration.GetConnectionString("DefaultConnection"), optionsBuilder => optionsBuilder.EnableRetryOnFailure()
       ));
builder.Services.AddHttpContextAccessor();
builder.Services.AddIdentity<ApplicationUser, BugTracker.Data.Entities.ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// bottom Use might be unness?
//app.Use(async (context, next) =>
//{
//    context.Response.GetTypedHeaders().CacheControl =
//        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
//        {
//            NoStore = true,
//            MaxAge = TimeSpan.Zero
//        };
//    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Pragma] = "no-cache";
//    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] = "-1";

//    await next();
//});
//app.Use(async (context, next) =>
//{
//    context.Response.GetTypedHeaders().CacheControl =
//        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
//        {
//            NoCache = true,
//            NoStore = true,
//            MustRevalidate = true
//        };
//    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Pragma] =
//        new string[] { "no-cache" };
//    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] =
//        new string[] { "0" };
//    await next();
//});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/");
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    // Add this block of code to apply migrations
    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
    await SeedRoles(serviceProvider);
}
app.Run();

async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<BugTracker.Data.Entities.ApplicationRole>>();

    var roles = new List<BugTracker.Data.Entities.ApplicationRole>
    {
        new BugTracker.Data.Entities.ApplicationRole { Name = "Admin" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Project_Manager" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Developer" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Submitter" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Demo_Admin" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Demo_Project_Manager" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Demo_Developer" },
        new BugTracker.Data.Entities.ApplicationRole { Name = "Demo_Submitter" },
        // Add other roles as needed
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.Name))
        {
            await roleManager.CreateAsync(role);
        }
    }
}
