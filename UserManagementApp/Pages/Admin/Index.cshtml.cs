using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .OrderByDescending(u => u.LastLoginTime) // Sort by LastLoginTime in descending order
                .ToListAsync();
        }

        public class ActionRequest
        {
            public string Action { get; set; } = string.Empty;
            public List<string> UserIds { get; set; } = new List<string>();
        }

        public async Task<IActionResult> OnPostPerformActionAsync()
        {
            _logger.LogInformation("Received request to perform action");

            try
            {
                // Read and deserialize the request body
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation($"Request Body: {body}");

                var requestData = JsonSerializer.Deserialize<ActionRequest>(body);
                if (requestData == null || requestData.UserIds == null || !requestData.UserIds.Any())
                {
                    _logger.LogWarning("Invalid request data received.");
                    return BadRequest("Invalid request data: No users selected.");
                }

                _logger.LogInformation("Action: {Action}, UserIds: {UserIds}", requestData.Action, string.Join(", ", requestData.UserIds));

                // Fetch the users to be updated
                var users = await _context.Users
                    .Where(u => requestData.UserIds.Contains(u.Id))
                    .ToListAsync();

                if (!users.Any())
                {
                    _logger.LogWarning("No matching users found.");
                    return NotFound("No matching users found.");
                }

                // Perform the action
                foreach (var user in users)
                {
                    switch (requestData.Action)
                    {
                        case "Block":
                            user.Status = "Blocked";
                            break;
                        case "Unblock":
                            user.Status = "Active";
                            break;
                        case "Delete":
                            _context.Users.Remove(user);
                            break;
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = $"{requestData.Action} action completed successfully." });
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization error: {ex.Message}");
                return BadRequest("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error performing action: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}