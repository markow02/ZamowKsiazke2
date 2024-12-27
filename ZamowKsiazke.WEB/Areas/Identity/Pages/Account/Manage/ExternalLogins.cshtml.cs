#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ZamowKsiazke.Models;

namespace ZamowKsiazke.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<DefaultUser> _userManager;
        private readonly SignInManager<DefaultUser> _signInManager;
        private readonly IUserStore<DefaultUser> _userStore;
        private readonly ILogger<ExternalLoginsModel> _logger;

        public ExternalLoginsModel(
            UserManager<DefaultUser> userManager,
            SignInManager<DefaultUser> signInManager,
            IUserStore<DefaultUser> userStore,
            ILogger<ExternalLoginsModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _logger = logger;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationScheme> OtherLogins { get; set; }
        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Unable to load user with ID '{UserId}'", _userManager.GetUserId(User));
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                CurrentLogins = await _userManager.GetLoginsAsync(user);
                OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                    .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                    .ToList();

                string passwordHash = null;
                if (_userStore is IUserPasswordStore<DefaultUser> userPasswordStore)
                {
                    passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
                }

                ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;

                _logger.LogInformation("External logins loaded for user {UserId}", _userManager.GetUserId(User));
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading external logins for user {UserId}", _userManager.GetUserId(User));
                throw;
            }
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found when attempting to remove external login.");
                    return NotFound("Unable to load user.");
                }

                var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to remove external login for user {UserId}", user.Id);
                    StatusMessage = "The external login was not removed.";
                    return RedirectToPage();
                }

                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User {UserId} removed an external login.", user.Id);
                StatusMessage = "The external login was removed.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing login for user {UserId}", _userManager.GetUserId(User));
                throw;
            }
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            try
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));

                _logger.LogInformation("Initiating linking of login {Provider} for user {UserId}", provider, _userManager.GetUserId(User));
                return new ChallengeResult(provider, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while linking login for user {UserId}", _userManager.GetUserId(User));
                throw;
            }
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Unable to load user with ID '{UserId}' for login linking callback", _userManager.GetUserId(User));
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var userId = await _userManager.GetUserIdAsync(user);
                var info = await _signInManager.GetExternalLoginInfoAsync(userId);
                if (info == null)
                {
                    throw new InvalidOperationException($"Unexpected error occurred loading external login info.");
                }

                var result = await _userManager.AddLoginAsync(user, info);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to add external login for user {UserId}", userId);
                    StatusMessage = "The external login was not added. External logins can only be associated with one account.";
                    return RedirectToPage();
                }

                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                StatusMessage = "The external login was added.";

                _logger.LogInformation("Added external login for user {UserId}", userId);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the external login callback for user {UserId}", _userManager.GetUserId(User));
                throw;
            }
        }
    }
}
