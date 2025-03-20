using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserManagementApp.Models;

namespace UserManagementApp.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            // Check if the user is authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user == null || user.Status == "Blocked")
                {
                    // Redirect blocked or deleted users to the login page
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}