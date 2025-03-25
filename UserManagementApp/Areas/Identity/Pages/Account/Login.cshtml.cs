#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using UserManagementApp.Models;

namespace UserManagementApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager; // Add UserManager
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, // Inject UserManager
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; // Initialize UserManager
            _logger = logger;

            // Initialize properties to avoid null warnings
            Input = new InputModel();
            ExternalLogins = new List<AuthenticationScheme>();
            ReturnUrl = string.Empty;
            ErrorMessage = string.Empty;
            LogoutMessage = string.Empty;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string LogoutMessage { get; set; } // Add TempData for logout message

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty; // Ensure it's initialized

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty; // Ensure it's initialized

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Check for both logout AND registration messages
            if (!string.IsNullOrEmpty(TempData["Message"] as string))
            {
                ViewData["Message"] = TempData["Message"];
            }
            else if (!string.IsNullOrEmpty(LogoutMessage))
            {
                ViewData["Message"] = LogoutMessage;
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).Wait();
            ExternalLogins = _signInManager.GetExternalAuthenticationSchemesAsync().Result.ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                
                // Explicit null check before accessing user properties
                if (user is not null && user.Status == "Blocked")
                {
                    ModelState.AddModelError(string.Empty, "Your account is blocked.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email, 
                    Input.Password, 
                    Input.RememberMe, 
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Safe to access user here because PasswordSignInAsync succeeded
                    var updatedUser = await _userManager.FindByEmailAsync(Input.Email);
                    if (updatedUser is not null)
                    {
                        updatedUser.LastLoginTime = DateTime.UtcNow;
                        await _userManager.UpdateAsync(updatedUser);
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                
                // More specific error messages
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Account not found.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid password.");
                }
            }

            return Page();
        }
    }
}