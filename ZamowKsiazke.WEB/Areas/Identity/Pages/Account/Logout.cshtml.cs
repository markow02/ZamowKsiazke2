﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;
using ZamowKsiazke.Services.Extensions;

namespace ZamowKsiazke.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<DefaultUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IUserActivityService _activityService;
        private readonly UserManager<DefaultUser> _userManager;

        public LogoutModel(
            SignInManager<DefaultUser> signInManager, 
            ILogger<LogoutModel> logger,
            IUserActivityService activityService,
            UserManager<DefaultUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _activityService = activityService;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                await _activityService.LogLogoutAsync(userId);
            }
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Redirect to the Store page after logout
                return RedirectToPage("/Store/Index");
            }
        }
    }
}
