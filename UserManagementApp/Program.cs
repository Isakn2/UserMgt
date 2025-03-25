using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Middleware;
using UserManagementApp.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity to accept any non-empty password
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    
    // Password settings (minimal requirements)
    options.Password.RequiredLength = 1;       // Allow 1-character passwords
    options.Password.RequireDigit = false;     // No digits required
    options.Password.RequireLowercase = false; // No lowercase required
    options.Password.RequireUppercase = false; // No uppercase required
    options.Password.RequireNonAlphanumeric = false; // No symbols required
})
.AddRoles<IdentityRole>() // To use roles 
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Add the middleware to the pipeline
app.UseMiddleware<UserStatusMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserStatusMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();