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

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            // Skip middleware for login/register pages
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Login") || 
                context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
            {
                await _next(context);
                return;
            }

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);
                
                if (user == null || user.Status == "Blocked")
                {
                    await signInManager.SignOutAsync();
                    context.Response.Redirect("/Identity/Account/Login?returnUrl=" + 
                        Uri.EscapeDataString(context.Request.Path));
                    return;
                }
            }

            await _next(context);
        }
    }
}